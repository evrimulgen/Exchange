using System.Linq;
using Exchange.Core.Interfaces;
using Exchange.Core.Models;
using Exchange.Cryptopia.APIResults;
using Exchange.Cryptopia.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Exchange.Cryptopia
{
    public interface ICryptopiaService
    {
        Task<CryptopiaOrderBook> GetMarketOrdersAsync(string marketName);
        Task<ICurrencyCoin> GetMarketAsync(string symbol);
        Task<IEnumerable<ICurrencyCoin>> GetMarketsAsync();
        Task<IEnumerable<CryptopiaOrderBook>> GetMarketOrderGroupsAsync(string[] marketNames);
        OrderBook GetOrderBook(string marketName);
        double GetMarketVolume(string marketName);

    }
    public class CryptopiaService : ICryptopiaService
    {
        private readonly IApiService _cryptopiaClient;

        public CryptopiaService()
        {
            _cryptopiaClient = new ApiService("https://www.cryptopia.co.nz/api/");
        }

        /// <summary>
        /// Request: https://www.cryptopia.co.nz/api/GetMarket/DOT_BTC
        /// Required: none
        /// Optional: hours, default 24
        /// Response: 
        ///      "TradePairId":100,
        ///      "Label":"LTC/BTC",
        ///      "AskPrice":0.00006000,
        ///      "BidPrice":0.02000000,
        ///      "Low":0.00006000,
        ///      "High":0.00006000,
        ///      "Volume":1000.05639978,
        ///      "LastPrice":0.00006000,
        ///      "BuyVolume":34455.678,
        ///      "SellVolume":67003436.37658233,
        ///      "Change":-400.00000000,
        ///      "Open": 0.00000500,
        ///      "Close": 0.00000600,
        ///      "BaseVolume": 3.58675866,
        ///      "BaseBuyVolume": 11.25364758,
        ///      "BaseSellVolume": 3456.06746543
        /// </summary>
        /// <returns>Task<ICurrencyCoin></returns>
        public async Task<ICurrencyCoin> GetMarketAsync(string symbol)
        {
            var result = await _cryptopiaClient.GetAsync<GetMarketResults>("GetMarket/" + symbol);

            if (result == null)
            {
                throw new NullReferenceException();
            }

            return result.Data;
        }

        /// <summary>
        /// Request: https://www.cryptopia.co.nz/api/GetMarkets 
        /// Request: https://www.cryptopia.co.nz/api/GetMarkets/BTC
        /// Required: none
        /// Optional: baseMarket, default all
        /// Optional: hours, default 24
        /// Response: 
        ///      "TradePairId":100,
        ///      "Label":"LTC/BTC",
        ///      "AskPrice":0.00006000,
        ///      "BidPrice":0.02000000,
        ///      "Low":0.00006000,
        ///      "High":0.00006000,
        ///      "Volume":1000.05639978,
        ///      "LastPrice":0.00006000,
        ///      "BuyVolume":34455.678,
        ///      "SellVolume":67003436.37658233,
        ///      "Change":-400.00000000,
        ///      "Open": 0.00000500,
        ///      "Close": 0.00000600,
        ///      "BaseVolume": 3.58675866,
        ///      "BaseBuyVolume": 11.25364758,
        ///      "BaseSellVolume": 3456.06746543
        /// </summary>
        /// <returns> Task<IEnumerable<ICurrencyCoin>></returns>
        public async Task<IEnumerable<ICurrencyCoin>> GetMarketsAsync()
        {
            var result = await _cryptopiaClient.GetAsync<GetMarketsResult>("GetMarkets");

            if (result == null)
            {
                throw new NullReferenceException();
            }

            return result.Data;
        }

        /// <summary>
        /// Request: https://www.cryptopia.co.nz/api/GetMarketOrders/100 
        /// Required: market, (TradePairId or MarketName)
        /// Optional: orderCount, default 100
        /// Response: 
        ///     "Buy":[{
        ///                 "TradePairId":100,
        ///                 "Label":"LTC/BTC",
        ///                 "Price":0.00006000,
        ///                 "Volume":455055.00360000,
        ///                 "Total":27.303300
        ///            },
        ///            { 
        ///               ...
        ///            }],
        ///     "Sell":[{
        ///                 "TradePairId":100,
        ///                 "Label":"LTC/BTC",
        ///                 "Price":0.02000000,
        ///                 "Volume":499.99640000,
        ///                 "Total":9.999928
        ///             },
        ///      }],
        /// </summary>
        /// <returns>Task<CryptopiaOrderBook></returns>
        public async Task<CryptopiaOrderBook> GetMarketOrdersAsync(string marketName)
        {
            var result = await _cryptopiaClient.GetAsync<GetMarketOrderResults>(string.Format("GetMarketOrders/{0}", marketName));

            if (result == null)
            {
                throw new NullReferenceException();
            }

            return result.Data;
        }

        /// <summary>
        /// Request: https://www.cryptopia.co.nz/api/GetMarketOrderGroups/100-101-102-103
        /// Response: 
        ///     {
        ///     "TradePairId": 100,
        ///     "Market": "LTC_BTC"
        ///     	"Buy":[orders...],
        ///     	"Sell":[orders...]
        ///     },
        ///     {
        ///     "TradePairId": 101,
        ///     "Market": "DOT_BTC"
        ///     	"Buy":[orders...],
        ///         "Sell":[orders...],
        ///      },
        ///      {
        ///         "TradePairId": 102,
        ///         "Market": "DOGE_BTC"
        ///         "Buy":[orders...],
        ///         "Sell":[orders...],
        ///      }
        /// </summary>
        /// <returns>Task<IEnumerable<CryptopiaOrderBook>></returns>
        public async Task<IEnumerable<CryptopiaOrderBook>> GetMarketOrderGroupsAsync(string[] marketNames)
        {
            var result = await _cryptopiaClient.GetAsync<GetMarketOrderGroupsResults>(string.Format("GetMarketOrderGroups/{0}", string.Join('-', marketNames)));

            if (result == null)
            {
                throw new NullReferenceException();
            }

            return result.Data;
        }

        public OrderBook GetOrderBook(string marketName)
        {
            var orderBook = new OrderBook();
            try
            {
                var result = GetMarketOrdersAsync(marketName).Result;
                orderBook.Buy = result.Buy.Select(c => new Order { Price = c.Price, Volume = c.Volume });
                orderBook.Sell = result.Sell.Select(c => new Order { Price = c.Price, Volume = c.Volume });
            }
            catch (System.AggregateException e)
            {

            }
            return orderBook;

        }

        public double GetMarketVolume(string marketName)
        {
            var result = GetMarketAsync(marketName).Result;
            return result.Volume;
        }
    }
}
