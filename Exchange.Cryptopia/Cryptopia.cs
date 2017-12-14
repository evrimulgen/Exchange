using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Exchange.Core.Interfaces;
using System.Text.RegularExpressions;
using Exchange.Core.Models;
using System.Net;
using System.Threading;
using Exchange.Cryptopia.APIResults;
using Exchange.Cryptopia.Model;

namespace Exchange.Cryptopia
{
    public interface ICryptopiaService
    {
        Task<OrderBook> GetMarketOrders(string marketName);
        Task<ICurrencyCoin> Get24hrAsync(string symbol);
        Task<IEnumerable<ICurrencyCoin>> ListPrices();
        Task<OrderBook> GetMarketOrderGroups(string[] marketNames);
    }
    public class CryptopiaService : ICryptopiaService
    {
        private readonly IApiService _cryptopiaClient;
        private string[] KNOWN_BAD_COINS = new string[] { "QTUM", "BTG", "FUEL" };

        public CryptopiaService()
        {
            _cryptopiaClient = new ApiService("https://www.cryptopia.co.nz/api/");
        }
        public async Task<ICurrencyCoin> Get24hrAsync(string symbol)
        {
            var result = await _cryptopiaClient.GetAsync<GetMarketResults>("GetMarket/" + symbol);

            if (result == null)
            {
                throw new NullReferenceException();
            }

            return result.Data;
        }

        public async Task<IEnumerable<ICurrencyCoin>> ListPrices()
        {
            var result = await _cryptopiaClient.GetAsync<GetMarketsResult>("GetMarkets");

            if (result == null)
            {
                throw new NullReferenceException();
            }

            return result.Data;
        }

        public async Task<OrderBook> GetMarketOrders(string marketName)
        {
            var result = await _cryptopiaClient.GetAsync<GetMarketOrderResults>(string.Format("GetMarketOrders/{0}", marketName));

            if (result == null)
            {
                throw new NullReferenceException();
            }

            return result.Data;
        }

        public async Task<OrderBook> GetMarketOrderGroups(string[] marketNames)
        {
            var result = await _cryptopiaClient.GetAsync<GetMarketOrderResults>(string.Format("GetMarketOrderGroups/{0}", string.Join('-', marketNames)));

            if (result == null)
            {
                throw new NullReferenceException();
            }

            return result.Data;
        }
    }
}
