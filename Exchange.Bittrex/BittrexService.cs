using System.Linq;
using Exchange.Bittrex.APIResults;
using Exchange.Bittrex.Model;
using Exchange.Core.Interfaces;
using Exchange.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Exchange.Bittrex
{
    public interface IBittrexService
    {
        Task<GetOrderBookResponse> GetOrderBookAsync(string marketName);
        Task<IEnumerable<ICurrencyCoin>> GetMarketSummariesAsync();
        Task<BittrexCoin> GetMarketSummaryAsync(string symbol);
        Task<IEnumerable<BittrexMarkets>> GetMarketsAsync();
        OrderBook GetOrderBook(string marketName);
    }

    public class BittrexService : IBittrexService
    {
        private readonly IApiService _bittrexClient;

        public BittrexService()
        {
            _bittrexClient = new ApiService("https://bittrex.com/api/");
        }
        /// <summary>
        /// Request: https://bittrex.com/api/v1.1/public/getmarkets    
        /// Response: 
        ///     "MarketCurrency" : "LTC",
		///     "BaseCurrency" : "BTC",
		///     "MarketCurrencyLong" : "Litecoin",
		///     "BaseCurrencyLong" : "Bitcoin",
		///     "MinTradeSize" : 0.01000000,
		///     "MarketName" : "BTC-LTC",
		///     "IsActive" : true,
		///     "Created" : "2014-02-13T00:00:00"
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<BittrexMarkets>> GetMarketsAsync()
        {
            var result = await _bittrexClient.GetAsync<GetMarketsResult>("v1.1/public/getmarkets");

            if (result == null)
            {
                throw new NullReferenceException();
            }

            return result.result;
        }

        /// <summary>
        /// Request: https://bittrex.com/api/v1.1/public/getticker
        /// Required: symbol
        /// Response: SINGLE:
        ///     "Bid" : 2.05670368,
		///     "Ask" : 3.35579531,
		///     "Last" : 3.35579531
        /// </summary>
        /// <returns>Task<BittrexTicker></returns>
        public async Task<BittrexTicker> GetTickerAsync(string symbol)
        {
            var result = await _bittrexClient.GetAsync<GetTickerResults>("v1.1/public/getticker", "market=" + symbol);

            if (result == null)
            {
                throw new NullReferenceException();
            }

            return result.result;
        }

        /// <summary>
        /// Request: https://bittrex.com/api/v1.1/public/getmarketsummaries    
        /// Required: none
        /// Response: LIST:
        ///     "MarketName" : "BTC-888",
        ///     "High" : 0.00000919,
        ///     "Low" : 0.00000820,
        ///     "Volume" : 74339.61396015,
        ///     "Last" : 0.00000820,
        ///     "BaseVolume" : 0.64966963,
        ///     "TimeStamp" : "2014-07-09T07:19:30.15",
        ///     "Bid" : 0.00000820,
        ///     "Ask" : 0.00000831,
        ///     "OpenBuyOrders" : 15,
        ///     "OpenSellOrders" : 15,
        ///     "PrevDay" : 0.00000821,
        ///     "Created" : "2014-03-20T06:00:00",
        ///     "DisplayMarketName" : null
        /// </summary>
        /// <returns>Task<IEnumerable<ICurrencyCoin>></returns>
        public async Task<IEnumerable<ICurrencyCoin>> GetMarketSummariesAsync()
        {
            var result = await _bittrexClient.GetAsync<GetMarketSummariesResult>("v1.1/public/getmarketsummaries");

            if (result == null)
            {
                throw new NullReferenceException();
            }

            return result.result;
        }

        /// <summary>
        /// Request: https://bittrex.com/api/v1.1/public/getmarketsummary?market=btc-ltc    
        /// Required: market
        /// Response: LIST:
        ///     "MarketName" : "BTC-888",
        ///     "High" : 0.00000919,
        ///     "Low" : 0.00000820,
        ///     "Volume" : 74339.61396015,
        ///     "Last" : 0.00000820,
        ///     "BaseVolume" : 0.64966963,
        ///     "TimeStamp" : "2014-07-09T07:19:30.15",
        ///     "Bid" : 0.00000820,
        ///     "Ask" : 0.00000831,
        ///     "OpenBuyOrders" : 15,
        ///     "OpenSellOrders" : 15,
        ///     "PrevDay" : 0.00000821,
        ///     "Created" : "2014-03-20T06:00:00",
        ///     "DisplayMarketName" : null
        /// </summary>
        /// <returns>Task<BittrexCoin></returns>
        public async Task<BittrexCoin> GetMarketSummaryAsync(string symbol)
        {
            var result = await _bittrexClient.GetAsync<BittrexCoin>("v1.1/public/getmarketsummary", "market=" + symbol);

            if (result == null)
            {
                throw new NullReferenceException();
            }

            return result;
        }

        /// <summary>
        /// Request: https://bittrex.com/api/v1.1/public/getorderbook?market=BTC-LTC&type=both    
        /// Required: market, type [buy, sell, both]
        /// Response: LIST:
        ///      "buy" : [
        ///   {
		///		"Quantity" : 12.37000000,
		///		"Rate" : 0.02525000
        ///    }
		/// ],
		/// "sell" : [{
		///		"Quantity" : 32.55412402,
		///		"Rate" : 0.02540000
        ///  }, {
		///		"Quantity" : 60.00000000,
		///		"Rate" : 0.02550000
		///	}, 
        ///	{
		///		"Quantity" : 60.00000000,
		///		"Rate" : 0.02575000
		///	}, 
        ///	{
		///		"Quantity" : 84.00000000,
		///		"Rate" : 0.02600000
		///	}
		///]
        /// </summary>
        /// <returns>Task<OrderBook></returns>
        public async Task<GetOrderBookResponse> GetOrderBookAsync(string marketName)
        {
            var result = await _bittrexClient.GetAsync<GetOrderBookResponse>("v1.1/public/getorderbook", "market=" + marketName + "&type=both");

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
                var result = GetOrderBookAsync(marketName).Result.result;
                orderBook.Buy = result.buy.Select(c => new Order { Price = c.Rate, Volume = c.Quantity });
                orderBook.Sell = result.sell.Select(c => new Order { Price = c.Rate, Volume = c.Quantity });
            }
            catch (System.AggregateException e)
            {

            }
            return orderBook;

        }
    }
}
