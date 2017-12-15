using Exchange.Binance.Model;
using Exchange.Core.Interfaces;
using Exchange.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Exchange.Binance
{
    public interface IBinanceService
    {
        Task<IEnumerable<ICurrencyCoin>> GetAllPricesAsync();
        Task<BinanceMarketResult> Get24hrAsync(string symbol);
        Task<BinanceOrderBook> GetMarketOrdersAsync(string marketName);
        OrderBook GetOrderBook(string marketName);
    }

    public class BinanceService : IBinanceService
    {
        private readonly IApiService _apiService;

        public BinanceService()
        {
            _apiService = new ApiService("https://www.binance.com/api/");
        }

        /// <summary>
        /// Request: https://www.binance.com/api/v1/ticker/24hr
        /// Required: symbol
        /// Optional:
        /// Response: 
        ///{
        ///  "priceChange": "-94.99999800",
        ///  "priceChangePercent": "-95.960",
        ///  "weightedAvgPrice": "0.29628482",
        ///  "prevClosePrice": "0.10002000",
        ///  "lastPrice": "4.00000200",
        ///  "bidPrice": "4.00000000",
        ///  "askPrice": "4.00000200",
        ///  "openPrice": "99.00000000",
        ///  "highPrice": "100.00000000",
        ///  "lowPrice": "0.10000000",
        ///  "volume": "8913.30000000",
        ///  "openTime": 1499783499040,
        ///  "closeTime": 1499869899040,
        ///  "fristId": 28385,   // First tradeId
        ///  "lastId": 28460,    // Last tradeId
        ///  "count": 76         // Trade count
        ///}
        /// </summary>
        /// <returns>Task<BinanceMarketResult></returns>
        public async Task<BinanceMarketResult> Get24hrAsync(string symbol)
        {
            var result = await _apiService.GetAsync<BinanceMarketResult>("v1/ticker/24hr", "symbol=" + symbol);

            if (result == null)
            {
                throw new NullReferenceException();
            }

            return result;
        }

        /// <summary>
        /// Request: https://www.binance.com/api/v1/ticker/24hr
        /// Required: none
        /// Optional:
        /// Response: 
        ///[
 		///		  {
		///		    "symbol": "LTCBTC",
 		///		    "price": "4.00000200"
        ///
        ///          },
		///		  {
		///		    "symbol": "ETHBTC",
		///		    "price": "0.07946600"
		///		  }
		///]
        /// </summary>
        /// <returns>Task<IEnumerable<ICurrencyCoin>></returns>
        public async Task<IEnumerable<ICurrencyCoin>> GetAllPricesAsync()
        {
            var result = await _apiService.GetAsync<IEnumerable<BinanceCoin>>("v1/ticker/allPrices");
            if (result == null)
            {
                throw new NullReferenceException();
            }
            return result;
        }

        /// <summary>
        /// Request: https://www.binance.com/api/v1/depth
        /// Required: symbol
        /// Optional: limit, default 100, max 100
        /// Response: 
        ///{
        ///  "lastUpdateId": 1027024,
        ///  "bids": [
        ///
        ///    [
        ///      "4.00000000",     // PRICE
        ///      "431.00000000",   // QTY
        ///
        ///      []                // Can be ignored
        ///    ]
        ///  ],
        ///  "asks": [
        ///
        ///    [
        ///      "4.00000200",
        ///      "12.00000000",
        ///
        ///      []
        ///    ]
        ///  ]
        ///}
        /// </summary>
        /// <returns>Task<BinanceOrderBook></returns>
        public async Task<BinanceOrderBook> GetMarketOrdersAsync(string marketName)
        {
            var result = await _apiService.GetAsync<BinanceOrderBook>("v1/depth", "symbol=" + marketName);
            if (result == null)
            {
                throw new NullReferenceException();
            }
            return result;
        }

        public OrderBook GetOrderBook(string marketName)
        {
            var orderBook = new OrderBook();
            try
            {
                var result = GetMarketOrdersAsync(marketName).Result;
                orderBook.Buy = result.bids.Select(c => new Order { Price = double.Parse(c.ElementAt(0).ToString()), Volume = double.Parse(c.ElementAt(1).ToString()) });
                orderBook.Sell = result.asks.Select(c => new Order { Price = double.Parse(c.ElementAt(0).ToString()), Volume = double.Parse(c.ElementAt(1).ToString()) });
            }
            catch (System.AggregateException e)
            {
                
            }
            return orderBook;

        }
    }
}
