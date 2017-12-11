using System.Collections.Generic;
using Exchange.Core.Interfaces;
using Exchange.Core.Models;
using Exchange.Services.Models;

namespace Exchange.Services
{
    public interface IExchangeNormalizerService
    {
        List<ArbitrageResult> GetArbitrageComparisions();
        IEnumerable<ICurrencyCoin> GetAllCoins();
        Dictionary<string, Dictionary<string, IEnumerable<ICurrencyCoin>>> GetExchangeComparison();
        OrderBook GetOrderBook(string exchange, string market, string symbol);
    }
}