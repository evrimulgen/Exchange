using Exchange.Services;
using System;
using System.Linq;

namespace Exchange.Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            while (true)
            {
                var exchangeService = new ExchangeNormalizerService();
                var results = exchangeService.Foo();

                foreach (var item in results)
                {
                    //Console.ForegroundColor = ConsoleColor.White;
                    //Console.WriteLine(string.Format("Found: {0}/{1} could be arbitraged between {2}({3}) and {4}({5}): {6}%",
                    //    item.Market,
                    //    item.Symbol,
                    //    item.Exchange1,
                    //    item.Exchange1Price.ToString("N8"),
                    //    item.Exchange2,
                    //    item.Exchange2Price.ToString("N8"),
                    //    item.Percentage.ToString("N2")
                    //    ));
                    var highPricedExchangeOrderBook = exchangeService.GetOrderBook(item.Exchange1, item.Market, item.Symbol);
                    var lowerPricedExchangeOrderBook = exchangeService.GetOrderBook(item.Exchange2, item.Market, item.Symbol);
                    if ((highPricedExchangeOrderBook != null && highPricedExchangeOrderBook.Sell.Any() && highPricedExchangeOrderBook.Buy.Any()) &&
                        (lowerPricedExchangeOrderBook != null && lowerPricedExchangeOrderBook.Sell.Any() && lowerPricedExchangeOrderBook.Buy.Any())
                        )
                    {
                        //Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Green;
                        //for (int i = 5; i >= 0; i--)
                        //{
                        //    Console.WriteLine(string.Format("Buy {0} @ {1} on {2}",
                        //    lowerPricedExchangeOrderBook.Sell.ElementAt(i).Volume.ToString("N8"),
                        //    lowerPricedExchangeOrderBook.Sell.ElementAt(i).Price.ToString("N8"),
                        //    item.Exchange2
                        //    ));
                        //}
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
                        //Console.WriteLine("");
                        //for (int i = 0; i < 6; i++)
                        //{
                        //    Console.WriteLine(string.Format("Sell {0} @ {1} on {2}",
                        //        highPricedExchangeOrderBook.Buy.ElementAt(i).Volume.ToString("N8"),
                        //        highPricedExchangeOrderBook.Buy.ElementAt(i).Price.ToString("N8"),
                        //        item.Exchange1
                        //        ));
                        //}
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\tCould not find any valid orders in the books for filling....");
                    }
                }

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Refreshing....");
                Console.ReadLine();
            }

        }
    }
}
