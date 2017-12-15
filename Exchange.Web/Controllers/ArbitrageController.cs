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
        private readonly IExchangeServices _exchangeService;

        public ArbitrageController(IExchangeServices exchangeService)
        {
            _exchangeService = exchangeService;
        }

        [HttpGet("[action]")]
        public IEnumerable<ArbitrageResult> Get()
        {

            var r1 = _exchangeService.GetAllExchangeTickersAndPrices();
            var r2 = _exchangeService.CreateExchangeComparison(r1);
            var r3 = _exchangeService.CalculateArbitrageFromComparison(r2);
            var result = r3.Select(c => new ArbitrageResult
            {
                Market = c.Item1.Market,
                Symbol = c.Item1.TickerSymbol,
                Exchange1 = c.Item1.Exchange,
                Exchange1Price = c.Item1.Price,
                Exchange2 = c.Item2.Exchange,
                Exchange2Price = c.Item2.Price,
                Exchange1Logo = c.Item1.Logo,
                Exchange2Logo = c.Item2.Logo
            });
            return result.OrderByDescending( c=> c.Percentage);
        }

        //[HttpGet("[action]")]
        //public ExchangeComparison Details(ArbitrageResult item)
        //{

        //    var highPricedExchangeOrderBook = _exchangeService.GetOrderBook(item.Exchange1, item.Market, item.Symbol);
        //    var lowerPricedExchangeOrderBook = _exchangeService.GetOrderBook(item.Exchange2, item.Market, item.Symbol);
        //    var result = new ExchangeComparison
        //    {
        //        Symbol = item.Symbol,
        //        Market = item.Market,
        //        High = new ExchangeCoin
        //        {
        //            Exchange = item.Exchange1,
        //            Market = item.Market,
        //            Symbol = item.Symbol,
        //            Orders = _exchangeService.GetOrderBook(item.Exchange1, item.Market, item.Symbol)
        //        },
        //        Low = new ExchangeCoin
        //        {
        //            Exchange = item.Exchange2,
        //            Market = item.Market,
        //            Symbol = item.Symbol,
        //            Orders = _exchangeService.GetOrderBook(item.Exchange2, item.Market, item.Symbol)
        //        },
        //    };
        //    return result;
        //}

        //private List<ArbitrageResult> Foo(List<ArbitrageResult> potentialArbs)
        //{
        //    var results = new List<ArbitrageResult>();
        //    //foreach(var pArb in potentialArbs)
        //    //{
        //    //    var comparison = GetExchangeComparison(pArb);
        //    //    if ((comparison.Low.Orders.MarketResult.Volume > 0 && comparison.High.Orders.MarketResult.Volume > 0) &&
        //    //        (comparison.Low.Orders.Sell.ElementAt(0).Price < comparison.High.Orders.Buy.ElementAt(0).Price))
        //    //        results.Add(pArb);
        //    //}
        //    return results;
        //}


        //private ExchangeComparison GetExchangeComparison(ArbitrageResult item)
        //{
        //    var highPricedExchangeOrderBook = _exchangeService.GetOrderBook(item.Exchange1, item.Market, item.Symbol);
        //    var lowerPricedExchangeOrderBook = _exchangeService.GetOrderBook(item.Exchange2, item.Market, item.Symbol);
        //    var result = new ExchangeComparison
        //    {
        //        Symbol = item.Symbol,
        //        Market = item.Market,
        //        High = new ExchangeCoin
        //        {
        //            Exchange = item.Exchange1,
        //            Market = item.Market,
        //            Symbol = item.Symbol,
        //            Orders = _exchangeService.GetOrderBook(item.Exchange1, item.Market, item.Symbol)
        //        },
        //        Low = new ExchangeCoin
        //        {
        //            Exchange = item.Exchange2,
        //            Market = item.Market,
        //            Symbol = item.Symbol,
        //            Orders = _exchangeService.GetOrderBook(item.Exchange2, item.Market, item.Symbol)
        //        },
        //    };
        //    return result;
        //}
    }
}