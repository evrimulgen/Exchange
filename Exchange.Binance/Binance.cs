using Exchange.Binance.Model;
using Exchange.Core.Interfaces;
using Exchange.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exchange.Binance
{
    public interface IBinanceService
    {
        Task<IEnumerable<ICurrencyCoin>> GetAllPricesAsync();
        Task<BinanceMarketResult> Get24hrAsync(string symbol);
        Task<BinanceOrderBook> GetMarketOrders(string marketName);
    }

    public class BinanceService : IBinanceService
    {
        private readonly IApiService _apiService;

        public BinanceService()
        {
            _apiService = new ApiService("https://www.binance.com/api/");
        }

        public async Task<BinanceMarketResult> Get24hrAsync(string symbol)
        {
            var result = await _apiService.GetAsync<BinanceMarketResult>("v1/ticker/24hr", "symbol=" + symbol);

            if (result == null)
            {
                throw new NullReferenceException();
            }

            return result;
        }

        public async Task<IEnumerable<ICurrencyCoin>> GetAllPricesAsync()
        {
            var result = await _apiService.GetAsync<IEnumerable<BinanceCoin>>("v1/ticker/allPrices");
            if (result == null)
            {
                throw new NullReferenceException();
            }
            return result;
        }

        public async Task<BinanceOrderBook> GetMarketOrders(string marketName)
        {
            var result = await _apiService.GetAsync<BinanceOrderBook>("v1/depth", "symbol=" + marketName);
            if (result == null)
            {
                throw new NullReferenceException();
            }
            return result;
        }
    }
}
