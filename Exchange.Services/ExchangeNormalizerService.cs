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

namespace Exchange.Services
{
    public class ExchangeNormalizerService
    {
        private IBinanceClient binanceClient;
        private IBittrexClient bittrexClient;
        private ICryptopiaClient cryptopiaClient;
        private IExchangeService binanceService;
        private IExchangeService bittrexService;
        private IExchangeService cryptopiaService;

        public ExchangeNormalizerService()
        {
            this.binanceClient = new BinanceClient();
            this.binanceService = new BinanceService(binanceClient);
            this.bittrexClient = new BittrexClient();
            this.bittrexService = new BittrexService(bittrexClient);
            this.cryptopiaClient = new CryptopiaClient();
            this.cryptopiaService = new CryptopiaService(cryptopiaClient);
        }
        public IEnumerable<ICurrencyCoin> GetAllCoins()
        {
            var list = new List<ICurrencyCoin>();
            list.AddRange(binanceService.ListPrices());
            list.AddRange(bittrexService.ListPrices());
            list.AddRange(cryptopiaService.ListPrices());
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
                    ob = cryptopiaService.GetMarketOrders(string.Format(@"{0}_{1}", symbol, market));
                    break;
                case "Binance":
                    ob = binanceService.GetMarketOrders(string.Format(@"{0}{1}", symbol.ToUpper(), market.ToUpper()));
                    break;
                case "Bittrex":
                    ob = bittrexService.GetMarketOrders(string.Format(@"{0}-{1}", market.ToUpper(), symbol.ToUpper()));
                    break;
                default:
                    break;
            }
            return ob;
        }
    }
}
