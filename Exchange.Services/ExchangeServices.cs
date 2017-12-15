using Exchange.Binance;
using Exchange.Bittrex;
using Exchange.Core.Interfaces;
using Exchange.Cryptopia;
using Exchange.Cryptopia.APIResults;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Exchange.Services
{
    public interface IExchangeServices
    {
        List<ICurrencyCoin> GetAllExchangeTickersAndPrices();
        Dictionary<string, Dictionary<string, IEnumerable<ICurrencyCoin>>> CreateExchangeComparison(IEnumerable<ICurrencyCoin> coins);
        List<Tuple<ICurrencyCoin, ICurrencyCoin>> CalculateArbitrageFromComparison(Dictionary<string, Dictionary<string, IEnumerable<ICurrencyCoin>>> results);
    }
    public class ExchangeServices : IExchangeServices
    {
        private readonly ICryptopiaService _cryptopiaService;
        private readonly IBittrexService _bittrexService;
        private readonly IBinanceService _binanceService;

        private string[] CRYPTOPIA_KNOWN_BAD_COINS = new string[] { "QTUM", "BTG", "FUEL", "CMT" };

        private double ARB_THRESHOLD = 5.00;

        public ExchangeServices(
            ICryptopiaService cryptopiaService,
            IBittrexService bittrexService,
            IBinanceService binanceService
        )
        {
            _cryptopiaService = cryptopiaService;
            _bittrexService = bittrexService;
            _binanceService = binanceService;
        }


        /// <summary>
        /// Gets all the tickers and their last prices from each exchange.
        /// </summary>
        public List<ICurrencyCoin> GetAllExchangeTickersAndPrices()
        {
            var allExchangeTickers = new List<ICurrencyCoin>();
            var crptopiaTickers = _cryptopiaService.GetMarketsAsync();
            var bittrexTickers = _bittrexService.GetMarketSummariesAsync();
            var binanceTickers = _binanceService.GetAllPricesAsync();

            Task.WaitAll(new Task[] { crptopiaTickers, bittrexTickers, binanceTickers });
            allExchangeTickers.AddRange(crptopiaTickers.Result.Where(c => !CRYPTOPIA_KNOWN_BAD_COINS.Contains(c.TickerSymbol)));
            allExchangeTickers.AddRange(binanceTickers.Result);
            allExchangeTickers.AddRange(bittrexTickers.Result);

            return allExchangeTickers;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coins">List of all ticker sybmols at each exchange and their prices</param>
        /// <returns></returns>
        public Dictionary<string, Dictionary<string, IEnumerable<ICurrencyCoin>>> CreateExchangeComparison(IEnumerable<ICurrencyCoin> coins)
        {
            var result = new Dictionary<string, Dictionary<string, IEnumerable<ICurrencyCoin>>>();
            var distinctListOfMarkets = new List<string>();
            foreach (var coin in coins.GroupBy(c => c.Market))
            {
                distinctListOfMarkets.Add(coin.Key);
            }

            foreach (var market in distinctListOfMarkets)
            {
                var distinctSymbolsMarket = coins.Where(c => c.Market.Equals(market)).GroupBy(c => c.TickerSymbol);
                var tmp = new Dictionary<string, IEnumerable<ICurrencyCoin>>();
                foreach (var distinctSymbol in distinctSymbolsMarket)
                {
                    tmp.Add(distinctSymbol.Key, coins.Where(c => c.Market == market && c.TickerSymbol.Equals(distinctSymbol.Key)));
                }
                result.Add(market, tmp);
            }
            return result;
        }

        /// <summary>
        /// Enumerates all the unique markets and unique coins at each market to determine
        /// if there is the potential of an arbitrage.
        /// 
        /// This method currently is very slow
        /// 
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        public List<Tuple<ICurrencyCoin, ICurrencyCoin>> CalculateArbitrageFromComparison(Dictionary<string, Dictionary<string, IEnumerable<ICurrencyCoin>>> results)
        {
            var list = new List<Tuple<ICurrencyCoin, ICurrencyCoin>>();
            foreach (var market in results.Where(c => c.Value.Count() > 0))
            {
                foreach (var ticker in market.Value.Where(c => c.Value.Count() > 1))
                {
                    list.AddRange(CompareSingleCoinAtMultipleExchanges(ticker.Value));
                }
            }
            return list;
        }

        /// <summary>
        /// Takes the results of a single coin at N exchanges and calculates
        /// which exchanges have the potential for an arbitrage.
        /// </summary>
        /// <param name="p"></param>
        /// <returns>List<Tuple<>> of canidates for single coin and their exchanges</returns>
        private List<Tuple<ICurrencyCoin, ICurrencyCoin>> CompareSingleCoinAtMultipleExchanges(IEnumerable<ICurrencyCoin> p)
        {
            var list = new List<Tuple<ICurrencyCoin, ICurrencyCoin>>();
            for (var i = 0; i + 1 < p.Count(); i++)
            {
                for (var ii = i + 1; ii < p.Count(); ii++)
                {
                    var compare = CompareCoins(p.ElementAt(i), p.ElementAt(ii));
                    if (compare != null)
                    {
                        list.Add(compare);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Takes  two coins and determines if they should be potential
        /// arbitrage candidates.  Currenly based on ARB_THRESHOLD amount
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns>Returns null if not a canidate otherwise returns a Tuple with both coins.</returns>
        private Tuple<ICurrencyCoin, ICurrencyCoin> CompareCoins(ICurrencyCoin c1, ICurrencyCoin c2)
        {
            Tuple<ICurrencyCoin, ICurrencyCoin> result = null;
            var priceDiff = ((c2.Price - c1.Price) / Math.Abs(c1.Price)) * 100;
            if (priceDiff >= ARB_THRESHOLD)
            {
                c1 = GetCoinDetails(c1);
                c2 = GetCoinDetails(c2);
                if(c1.Volume > 0 && c2.Volume > 0)  
                    result = Tuple.Create<ICurrencyCoin, ICurrencyCoin>(c1, c2);
            }
            return result;
        }

        private ICurrencyCoin GetCoinDetails(ICurrencyCoin coin)
        {
            switch (coin.Exchange)
            {
                case "Binance":
                    var r1 = _binanceService.Get24hrAsync(coin.APIFormatted).Result;
                    coin.LastPrice = r1.lastPrice;
                    coin.AskPrice = r1.askPrice;
                    coin.BidPrice = r1.bidPrice;
                    coin.Volume = r1.volume;
                    break;
                case "Bittrex":
                    var r2 = _bittrexService.GetMarketSummaryAsync(coin.APIFormatted).Result;
                    coin.LastPrice = r2.LastPrice;
                    coin.AskPrice = r2.AskPrice;
                    coin.BidPrice = r2.BidPrice;
                    coin.Volume = r2.Volume;
                    break;
                case "Cryptopia":
                    var r3 = _cryptopiaService.GetMarketAsync(coin.APIFormatted).Result;
                    coin.LastPrice = r3.LastPrice;
                    coin.AskPrice = r3.AskPrice;
                    coin.BidPrice = r3.BidPrice;
                    coin.Volume = r3.Volume;
                    break;
                default:
                    break;
            }
            return coin;
        }
     }
}