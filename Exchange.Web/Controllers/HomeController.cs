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
        private readonly IExchangeNormalizerService _exchangeService;

        public HomeController(IExchangeNormalizerService exchangeService)
        {
            _exchangeService = exchangeService;
        }
        public IActionResult Index()
        {
            var t = _exchangeService.GetArbitrageComparisions();
            return View(t);
        }

        public IActionResult Details(ArbitrageResult item)
        {
            var highPricedExchangeOrderBook = _exchangeService.GetOrderBook(item.Exchange1, item.Market, item.Symbol);
            var lowerPricedExchangeOrderBook = _exchangeService.GetOrderBook(item.Exchange2, item.Market, item.Symbol);
            var result = new ExchangeComparison
            {
                Symbol = item.Symbol,
                Market = item.Market,
                High = new ExchangeCoin {
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
