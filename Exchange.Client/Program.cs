using Exchange.Binance;
using Exchange.Bittrex;
using Exchange.Cryptopia;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using static Exchange.Binance.BinanceClient;
using static Exchange.Bittrex.BittrexClient;
using static Exchange.Cryptopia.CryptopiaClient;
using Exchange.Services;
using Exchange.Core.Interfaces;

namespace Exchange.Client
{
    public class Program
    {

        public static void Main(string[] args)
        {
            while (true)
            {
                var exchangeService = new ExchangeNormalizerService();
                var t = exchangeService.GetExchangeComparison();
                var results = new List<ArbitrageResult>();

                foreach (var market in t)
                {
                    foreach (var symbol in market.Value)
                    {
                        results.AddRange(ShowComparison(symbol));
                    }
                }

                foreach (var item in results)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(string.Format("Found: {0}/{1} could be arbitraged between {2}({3}) and {4}({5}): {6}%",
                        item.Market,
                        item.Symbol,
                        item.Exchange1,
                        item.Exchange1Price.ToString("N8"),
                        item.Exchange2,
                        item.Exchange2Price.ToString("N8"),
                        item.Percentage.ToString("N2")
                        ));
                    var highPricedExchangeOrderBook = exchangeService.GetOrderBook(item.Exchange1, item.Market, item.Symbol);
                    var lowerPricedExchangeOrderBook = exchangeService.GetOrderBook(item.Exchange2, item.Market, item.Symbol);
                    if ((highPricedExchangeOrderBook != null && highPricedExchangeOrderBook.Sell.Any() && highPricedExchangeOrderBook.Buy.Any()) &&
                        (lowerPricedExchangeOrderBook != null && lowerPricedExchangeOrderBook.Sell.Any() && lowerPricedExchangeOrderBook.Buy.Any())
                        )
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(string.Format("\tBuy {0} @ {1} on {2}\n\tSell {3} @ {4} on {5}",
                            lowerPricedExchangeOrderBook.Buy.ElementAt(0).Volume.ToString("N8"),
                            lowerPricedExchangeOrderBook.Buy.ElementAt(0).Price.ToString("N8"),
                            item.Exchange2,
                            highPricedExchangeOrderBook.Sell.ElementAt(0).Volume.ToString("N8"),
                            highPricedExchangeOrderBook.Sell.ElementAt(0).Price.ToString("N8"),
                            item.Exchange1
                            ));
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\tCould not find any valid orders in the books for filling....");
                    }
                }

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Refreshing....");
                //Console.ReadLine();
            }

        }

        public static IEnumerable<ArbitrageResult> ShowComparison(KeyValuePair<string, IEnumerable<ICurrencyCoin>> coins)
        {
            var results = new List<ArbitrageResult>();
            switch (coins.Value.Count())
            {
                case 1:
                    break;
                case 2:
                    var a = ComparePrices(coins.Value.ElementAt(0), coins.Value.ElementAt(1));
                    if (a != null) results.Add(a);
                    break;
                case 3:
                    var b = ComparePrices(coins.Value.ElementAt(0), coins.Value.ElementAt(1));
                    var c = ComparePrices(coins.Value.ElementAt(1), coins.Value.ElementAt(2));
                    var d = ComparePrices(coins.Value.ElementAt(0), coins.Value.ElementAt(2));
                    if (b != null) results.Add(b);
                    if (c != null) results.Add(c);
                    if (d != null) results.Add(d);
                    break;
                case 4:
                    break;
                default:
                    break;
            }
            return results;
        }

        public static ArbitrageResult ComparePrices(ICurrencyCoin value1, ICurrencyCoin value2)
        {
            ArbitrageResult result = null;
            if (value1.Price > value2.Price && ((1 - (value2.Price / value1.Price)) * 100) > 5)
            {
                result = new ArbitrageResult
                {
                    Market = value1.Market,
                    Symbol = value1.TickerSymbol,
                    Exchange1 = value1.Exchange,
                    Exchange1Price = value1.Price,
                    Exchange2 = value2.Exchange,
                    Exchange2Price = value2.Price,
                    Percentage = ((1 - (value2.Price / value1.Price)) * 100)
                };

            }
            else if (((1 - (value1.Price / value2.Price)) * 100) > 5)
            {
                result = new ArbitrageResult
                {
                    Market = value1.Market,
                    Symbol = value1.TickerSymbol,
                    Exchange1 = value2.Exchange,
                    Exchange1Price = value2.Price,
                    Exchange2 = value1.Exchange,
                    Exchange2Price = value1.Price,
                    Percentage = ((1 - (value1.Price / value2.Price)) * 100)
                };
            }
            return result;

        }
    }

    public class ArbitrageResult
    {
        public string Market { get; set; }
        public string Symbol { get; set; }
        public string Exchange1 { get; set; }
        public double Exchange1Price { get; set; }
        public string Exchange2 { get; set; }
        public double Exchange2Price { get; set; }
        public double Percentage { get; set; }
    }


}
