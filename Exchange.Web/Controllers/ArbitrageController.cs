using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exchange.Core.Models;
using Exchange.Services;
using Exchange.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace Exchange.Web.Controllers
{
    [Route("api/[controller]")]
    public class ArbitrageController : Controller
    {
        private readonly IExchangeNormalizerService _exchangeService;

        public ArbitrageController(IExchangeNormalizerService exchangeService)
        {
            _exchangeService = exchangeService;
        }

        [HttpGet("[action]")]
        public IEnumerable<ArbitrageResult> Get()
        {
            var results = _exchangeService.GetArbitrageComparisions();
            if (!results.Any())
            {
                results = new List<ArbitrageResult>()
                {
                    new ArbitrageResult
                    {
                        Market = "BTC",
                        Symbol = "XXX",
                        Exchange1 = "Kyle",
                        Exchange1Price = 1.00,
                        Exchange2 = "Sarah",
                        Exchange2Price = 1.00,
                        Percentage = 50
                    }
                };
            }
            return Foo(results).OrderByDescending(c => c.Percentage);
        }

        [HttpGet("[action]")]
        public ExchangeComparison Details(ArbitrageResult item)
        {
            
            var highPricedExchangeOrderBook = _exchangeService.GetOrderBook(item.Exchange1, item.Market, item.Symbol);
            var lowerPricedExchangeOrderBook = _exchangeService.GetOrderBook(item.Exchange2, item.Market, item.Symbol);
            var result = new ExchangeComparison
            {
                Symbol = item.Symbol,
                Market = item.Market,
                High = new ExchangeCoin
                {
                    Exchange = item.Exchange1,
                    Market = item.Market,
                    Symbol = item.Symbol,
                    Orders = _exchangeService.GetOrderBook(item.Exchange1, item.Market, item.Symbol)
                },
                Low = new ExchangeCoin
                {
                    Exchange = item.Exchange2,
                    Market = item.Market,
                    Symbol = item.Symbol,
                    Orders = _exchangeService.GetOrderBook(item.Exchange2, item.Market, item.Symbol)
                },
            };
            return result;
        }

        private List<ArbitrageResult> Foo(List<ArbitrageResult> potentialArbs)
        {
            var results = new List<ArbitrageResult>();
            foreach(var pArb in potentialArbs)
            {
                var comparison = GetExchangeComparison(pArb);
                if ((comparison.Low.Orders.Market.Volume > 0 && comparison.High.Orders.Market.Volume > 0) &&
                    (comparison.Low.Orders.Sell.ElementAt(0).Price < comparison.High.Orders.Buy.ElementAt(0).Price))
                    results.Add(pArb);
            }
            return results;
        }


        private ExchangeComparison GetExchangeComparison(ArbitrageResult item)
        {
            var highPricedExchangeOrderBook = _exchangeService.GetOrderBook(item.Exchange1, item.Market, item.Symbol);
            var lowerPricedExchangeOrderBook = _exchangeService.GetOrderBook(item.Exchange2, item.Market, item.Symbol);
            var result = new ExchangeComparison
            {
                Symbol = item.Symbol,
                Market = item.Market,
                High = new ExchangeCoin
                {
                    Exchange = item.Exchange1,
                    Market = item.Market,
                    Symbol = item.Symbol,
                    Orders = _exchangeService.GetOrderBook(item.Exchange1, item.Market, item.Symbol)
                },
                Low = new ExchangeCoin
                {
                    Exchange = item.Exchange2,
                    Market = item.Market,
                    Symbol = item.Symbol,
                    Orders = _exchangeService.GetOrderBook(item.Exchange2, item.Market, item.Symbol)
                },
            };
            return result;
        }
    }
}