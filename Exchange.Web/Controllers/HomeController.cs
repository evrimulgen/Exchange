using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Exchange.Web.Models;
using Exchange.Services;
using Exchange.Services.Models;
using Exchange.Core.Models;

namespace Exchange.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var exchangeService = new ExchangeNormalizerService();
            var t = exchangeService.Foo();

            return View(t);
        }

        public IActionResult Details(ArbitrageResult item)
        {
            var exchangeService = new ExchangeNormalizerService();
            var highPricedExchangeOrderBook = exchangeService.GetOrderBook(item.Exchange1, item.Market, item.Symbol);
            var lowerPricedExchangeOrderBook = exchangeService.GetOrderBook(item.Exchange2, item.Market, item.Symbol);
            var result = new ExchangeComparison
            {
                Symbol = item.Symbol,
                Market = item.Market,
                High = new ExchangeCoin {
                    Exchange = item.Exchange1,
                    Market = item.Market,
                    Symbol = item.Symbol,
                    Orders = exchangeService.GetOrderBook(item.Exchange1, item.Market, item.Symbol)
                },
                Low = new ExchangeCoin
                {
                    Exchange = item.Exchange2,
                    Market = item.Market,
                    Symbol = item.Symbol,
                    Orders = exchangeService.GetOrderBook(item.Exchange2, item.Market, item.Symbol)
                },
            };
            return View(result);
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
