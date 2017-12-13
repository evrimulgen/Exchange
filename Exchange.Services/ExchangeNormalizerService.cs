using Exchange.Binance;
using Exchange.Bittrex;
using Exchange.Cryptopia;
using System;
using System.Linq;
using System.Collections.Generic;
using static Exchange.Binance.BinanceClient;
using static Exchange.Bittrex.BittrexClient;
using static Exchange.Cryptopia.CryptopiaClient;
using Exchange.Core.Interfaces;
using Exchange.Core.Models;
using Exchange.Services.Models;

namespace Exchange.Services
{
    public class ExchangeNormalizerService : IExchangeNormalizerService
    {
        private readonly IBinanceService _binanceService;
        private readonly IBittrexService _bittrexService;
        private readonly ICryptopiaService _cryptopiaService;

        public ExchangeNormalizerService(
            IBinanceService binanceService,
            IBittrexService bittrexService,
            ICryptopiaService cryptopiaService
            )
        {
            _binanceService = binanceService;
            _bittrexService = bittrexService;
            _cryptopiaService = cryptopiaService;
        }
        public IEnumerable<ICurrencyCoin> GetAllCoins()
        {
            var list = new List<ICurrencyCoin>();
            list.AddRange(_binanceService.ListPrices());
            list.AddRange(_bittrexService.ListPrices());
            list.AddRange(_cryptopiaService.ListPrices());
            return list;
        }

        public Dictionary<string, Dictionary<string, IEnumerable<ICurrencyCoin>>> GetExchangeComparison()
        {
            var result = new Dictionary<string, Dictionary<string, IEnumerable<ICurrencyCoin>>>();
            var allCoins = GetAllCoins().ToList();
            var distinctListOfMarkets = new List<string>();
            foreach(var coin in allCoins.GroupBy(c => c.Market))
            {
                distinctListOfMarkets.Add(coin.Key);
            }

            foreach(var market in distinctListOfMarkets)
            {
                var distinctSymbolsMarket = allCoins.Where(c => c.Market.Equals(market)).GroupBy(c => c.TickerSymbol);
                var tmp = new Dictionary<string, IEnumerable<ICurrencyCoin>>();
                foreach (var distinctSymbol in distinctSymbolsMarket)
                {
                    tmp.Add(distinctSymbol.Key, allCoins.Where(c => c.Market == market && c.TickerSymbol.Equals(distinctSymbol.Key)));
                }
                result.Add(market, tmp);
            }

            return result;
        }

        public OrderBook GetOrderBook(string exchange, string market, string symbol)
        {
            OrderBook ob = null;
            switch (exchange)
            {
                case "Cryptopia":
                    ob = _cryptopiaService.GetMarketOrders(string.Format(@"{0}_{1}", symbol, market));
                    var i = _cryptopiaService.Get24hrAsync(string.Format(@"{0}_{1}", symbol, market)).Result;
                    ob.Market = new MarketResult { Volume = i.Volume, Last = i.LastPrice };
                    break;
                case "Binance":
                    ob = _binanceService.GetMarketOrders(string.Format(@"{0}{1}", symbol.ToUpper(), market.ToUpper()));
                    var ii = _binanceService.Get24hrAsync(string.Format(@"{0}{1}", symbol.ToUpper(), market.ToUpper())).Result;
                    ob.Market = new MarketResult { Volume = ii.volume, Last = ii.lastPrice };
                    break;
                case "Bittrex":
                    ob = _bittrexService.GetMarketOrders(string.Format(@"{0}-{1}", market.ToUpper(), symbol.ToUpper()));
                    var iii = _bittrexService.Get24hrAsync(string.Format(@"{0}-{1}", market.ToUpper(), symbol.ToUpper())).Result;
                    ob.Market = new MarketResult { Volume = iii.Volume, Last = iii.Last };
                    break;
                default:
                    break;
            }
            return ob;
        }

        public List<ArbitrageResult> GetArbitrageComparisions()
        {
            var t = GetExchangeComparison();
            var results = new List<ArbitrageResult>();

            foreach (var market in t)
            {
                foreach (var symbol in market.Value)
                {
                    results.AddRange(ShowComparison(symbol));
                }
            }

            return results;
        }

        private ArbitrageResult ComparePrices(ICurrencyCoin value1, ICurrencyCoin value2)
        {
            ArbitrageResult result = null;
            if (value1.Price > value2.Price && ((1 - (value2.Price / value1.Price)) * 100) > 5)
            {
                result = new ArbitrageResult
                {
                    Market = value1.Market,
                    Symbol = value1.TickerSymbol,
                    Exchange1 = value1.Exchange,
                    Exchange1Logo = value1.Logo,
                    Exchange1Price = value1.Price,
                    Exchange2 = value2.Exchange,
                    Exchange2Logo = value2.Logo,
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
                    Exchange1Logo = value2.Logo,
                    Exchange1Price = value2.Price,
                    Exchange2 = value1.Exchange,
                    Exchange2Logo = value1.Logo,
                    Exchange2Price = value1.Price,
                    Percentage = ((1 - (value1.Price / value2.Price)) * 100)
                };
            }
            return result;

        }

        private IEnumerable<ArbitrageResult> ShowComparison(KeyValuePair<string, IEnumerable<ICurrencyCoin>> coins)
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

    }
}
