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
        IEnumerable<ICurrencyCoin> ListPrices();
        Task<BinanceMarketResult> Get24hrAsync(string symbol);
        OrderBook GetMarketOrders(string marketName);
        Task<dynamic> GetAllPricesAsync();
    }

    public class BinanceService : IBinanceService
    {
        private readonly IApiService _apiService;

        public BinanceService()
        {
            _apiService = new ApiService("https://www.binance.com/api/");
        }

        //Get depth of a symbol
        public async Task<dynamic> GetDepthAsync(string symbol)
        {
            var result = await _apiService.GetAsync<dynamic>("v1/depth", "symbol=" + symbol);

            if (result == null)
            {
                throw new NullReferenceException();
            }

            return result;
        }

        //Get depth of a symbol
        public async Task<BinanceMarketResult> Get24hrAsync(string symbol)
        {
            var result = await _apiService.GetAsync<BinanceMarketResult>("v1/ticker/24hr", "symbol=" + symbol);

            if (result == null)
            {
                throw new NullReferenceException();
            }

            return result;
        }

        //Get latest price of all symbols
        public async Task<dynamic> GetAllPricesAsync()
        {

            var result = await _apiService.GetAsync<dynamic>("v1/ticker/allPrices");
            if (result == null)
            {
                throw new NullReferenceException();
            }

            return result;

        }
        //Overload for ease of use
        public IEnumerable<ICurrencyCoin> ListPrices()
        {
            var prices = new List<BinanceCoin>();
            var task = Task.Run(async () => await _apiService.GetAsync<dynamic>("v1/ticker/allPrices"));
            dynamic result = task.Result;
            prices = JsonConvert.DeserializeObject<IEnumerable<BinanceCoin>>(result.ToString());
            return prices;
        }

        public OrderBook GetMarketOrders(string marketName)
        {
            var prices = new BinanceOrderBook();
            var task = Task.Run(async () => await _apiService.GetAsync<dynamic>("v1/depth", "symbol=" + marketName));
            dynamic result = task.Result;
            prices = JsonConvert.DeserializeObject<BinanceOrderBook>(result.ToString());
            return new OrderBook
            {
                Buy = prices.bids.Select(c => new Order { Price = double.Parse(c.ElementAt(0).ToString()), Volume = double.Parse(c.ElementAt(1).ToString()) }),
                Sell = prices.asks.Select(c => new Order { Price = double.Parse(c.ElementAt(0).ToString()), Volume = double.Parse(c.ElementAt(1).ToString()) }),
            };
        }
    }
}
