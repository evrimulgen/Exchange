using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Exchange.Core.Interfaces;
using Exchange.Core.Models;

namespace Exchange.Bittrex
{
    public interface IBittrexClient : IPublicExchangeClient
    {
    }

    public class BittrexClient : IBittrexClient
    {
        private readonly HttpClient _httpClient;
        private string url = "https://bittrex.com/api/";

        public BittrexClient()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(url)

            };

            _httpClient.DefaultRequestHeaders
                    .Accept
                    .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        }

        // GET
        public async Task<T> GetAsync<T>(string endpoint, string args = null)
        {
            var response = await _httpClient.GetAsync($"{endpoint}?{args}");

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(response.StatusCode.ToString());

            var result = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(result);
        }

        public interface IBittrexService 
        {
            OrderBook GetMarketOrders(string marketName);
            IEnumerable<ICurrencyCoin> ListPrices();
            Task<BittrexMarketResult> Get24hrAsync(string symbol);
        }

        public class BittrexService : IBittrexService
        {
            private readonly IBittrexClient _BittrexClient;

            public BittrexService(IBittrexClient binanceClient)
            {
                _BittrexClient = binanceClient;
            }

            public async Task<BittrexMarketResult> Get24hrAsync(string symbol)
            {
                var result = await _BittrexClient.GetAsync<BittrexMarketResult>("v1.1/public/getmarketsummary", "market=" + symbol);

                if (result == null)
                {
                    throw new NullReferenceException();
                }

                return result;
            }


            public OrderBook GetMarketOrders(string marketName)
            {
                var prices = new GetBookOrderResults();
                var task = Task.Run(async () => await _BittrexClient.GetAsync<dynamic>("v1.1/public/getorderbook", "market=" + marketName + "&type=both"));
                dynamic result = task.Result;
                prices = JsonConvert.DeserializeObject<GetBookOrderResults>(result.ToString());
                return new OrderBook
                {
                    Buy = prices.result == null || prices.result.buy == null ? new List<Order>() : prices.result.buy.Select(c => new Order { Price = c.Rate, Volume = c.Quantity }),
                    Sell = prices.result == null || prices.result.sell == null ? new List<Order>() : prices.result.sell.Select(c => new Order { Price = c.Rate, Volume = c.Quantity }),
                };
            }

            //Overload for ease of use
            public IEnumerable<ICurrencyCoin> ListPrices()
            {
                GetMarketSummariesResultRootObject priceList = new GetMarketSummariesResultRootObject();
                var task = Task.Run(async () => await _BittrexClient.GetAsync<dynamic>("v1.1/public/getmarketsummaries"));
                dynamic result = task.Result;
                priceList = JsonConvert.DeserializeObject<GetMarketSummariesResultRootObject>(result.ToString());
                return priceList.result;

            }
        }
    }


    public class GetBookOrderResults
    {
        public bool success { get; set; }
        public string message { get; set; }
        public Foo result { get; set; }
    }

    public class BittrexOrder
    {
        public double Quantity { get; set; }
        public double Rate { get; set; }
    }

    public class Foo
    {
        public List<BittrexOrder> buy { get; set; }
        public List<BittrexOrder> sell { get; set; }
    }

    public class BittrexMarketResult
    {
        //GET https://bittrex.com/api/v1.1/public/getmarketsummary?market=btc-ltc    
        public string URL { get { return "api/v1.1/public/getmarketsummary?market={0}"; } }
        public string MarketName { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Volume { get; set; }
        public double Last { get; set; }
        public double BaseVolume { get; set; }
        public DateTime TimeStamp { get; set; }
        public double Bid { get; set; }
        public double Ask { get; set; }
        public int OpenBuyOrders { get; set; }
        public int OpenSellOrders { get; set; }
        public double PrevDay { get; set; }
        public DateTime Created { get; set; }
        public object DisplayMarketName { get; set; }
    }

    //For Market API Call

    public class MarketResultEntry
    {
        public string MarketCurrency { get; set; }
        public string BaseCurrency { get; set; }
        public string MarketCurrencyLong { get; set; }
        public string BaseCurrencyLong { get; set; }
        public double MinTradeSize { get; set; }
        public string MarketName { get; set; }
        public bool IsActive { get; set; }
        public DateTime Created { get; set; }
        public string Notice { get; set; }
        public bool? IsSponsored { get; set; }
        public string LogoUrl { get; set; }
    }

    public class GetMarketsResult
    {
        public bool success { get; set; }
        public string message { get; set; }
        public List<MarketResultEntry> result { get; set; }
    }

    //For Summary API Call

    public class GetMarketSummariesResultRootObject
    {
        public bool success { get; set; }
        public string message { get; set; }
        public List<BittrexCoin> result { get; set; }
    }

    public class BittrexCoin : ICurrencyCoin
    {
        public string Exchange { get { return "Bittrex"; } }
        public string Logo {  get { return "https://bittrex.com/Content/img/logos/bittrex-96.png"; } }
        public string MarketName { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Volume { get; set; }
        public double Last { get; set; }
        public double BaseVolume { get; set; }
        public DateTime TimeStamp { get; set; }
        public double Bid { get; set; }
        public double Ask { get; set; }
        public int OpenBuyOrders { get; set; }
        public int OpenSellOrders { get; set; }
        public double PrevDay { get; set; }
        public DateTime Created { get; set; }
        public object DisplayMarketName { get; set; }
        public string TickerSymbol
        {
            get
            {
                return this.MarketName.Split('-')[1];
            }
        }

        public string Market
        {
            get
            {
                return this.MarketName.Split('-')[0];
            }
        }

        public double Price
        {
            get
            {
                return this.Last;
            }
        }
    }
}
