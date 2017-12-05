using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Exchange.Core.Interfaces;
using System.Text.RegularExpressions;
using Exchange.Core.Models;

namespace Exchange.Cryptopia
{
    public interface ICryptopiaClient : IPublicExchangeClient
    {
    }

    public class CryptopiaClient : ICryptopiaClient
    {
        private readonly HttpClient _httpClient;
        private string url = "https://www.cryptopia.co.nz/api/";
        
        public CryptopiaClient()
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

        public interface ICryptopiaService
        {
        }
        public class CryptopiaService : ICryptopiaService, IExchangeService
        {
            private readonly ICryptopiaClient _cryptopiaClient;
            private string[] KNOWN_BAD_COINS = new string[] { "QTUM", "BTG", "FUEL" };

            public CryptopiaService(ICryptopiaClient binanceClient)
            {
                _cryptopiaClient = binanceClient;
            }
            
            //Overload for ease of use
            public IEnumerable<ICurrencyCoin> ListPrices()
            {
                var priceList = new GetMarketsResult();
                var task = Task.Run(async () => await _cryptopiaClient.GetAsync<dynamic>("GetMarkets"));
                dynamic result = task.Result;
                priceList = JsonConvert.DeserializeObject<GetMarketsResult>(result.ToString());
                return priceList.Data.Where(c => !KNOWN_BAD_COINS.Contains(c.TickerSymbol));

            }

            public OrderBook GetMarketOrders(string marketName)
            {
                var priceList = new GetMarketOrderResults();
                var task = Task.Run(async () => await _cryptopiaClient.GetAsync<dynamic>(string.Format("GetMarketOrders/{0}", marketName)));
                dynamic result = task.Result;
                priceList = JsonConvert.DeserializeObject<GetMarketOrderResults>(result.ToString());
                return priceList.Data;
            }
        }
    }

    public class CryptopiaCoin : ICurrencyCoin
    {
        private Regex _labelRegex = new Regex(@"(?<symbol>.*)\/(?<market>.*)");
        public string Exchange { get { return "Cryptopia";  } }
        public int TradePairId { get; set; }
        public string Label { get; set; }
        public double AskPrice { get; set; }
        public double BidPrice { get; set; }
        public double Low { get; set; }
        public double High { get; set; }
        public double Volume { get; set; }
        public double LastPrice { get; set; }
        public double BuyVolume { get; set; }
        public double SellVolume { get; set; }
        public double Change { get; set; }
        public double Open { get; set; }
        public double Close { get; set; }
        public double BaseVolume { get; set; }
        public double BuyBaseVolume { get; set; }
        public double SellBaseVolume { get; set; }

        public string TickerSymbol
        {
            get
            {
                return _labelRegex.Match(this.Label).Groups["symbol"].ToString();
            }
        }
        public string Market
        {
            get
            {
                return _labelRegex.Match(this.Label).Groups["market"].ToString();
            }
        }

        public double Price { get { return LastPrice; } }
    }

    public class GetMarketsResult
    {
        public bool Success { get; set; }
        public object Message { get; set; }
        public IEnumerable<CryptopiaCoin> Data { get; set; }
        public object Error { get; set; }
    }

    public class GetMarketOrderResults
    {
        public bool Success { get; set; }
        public object Message { get; set; }
        public OrderBook Data { get; set; }
        public object Error { get; set; }
    }
}
