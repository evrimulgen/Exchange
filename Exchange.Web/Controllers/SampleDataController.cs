using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Exchange.Services;
using Exchange.Services.Models;

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
            if(!results.Any())
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
            return results.OrderByDescending(c => c.Percentage);
        }
        
    }
}
