using Exchange.Services;
using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Exchange.Cryptopia;
using Exchange.Binance;
using Exchange.Bittrex;
using System.Diagnostics;
using System.Collections.Generic;
using Exchange.Core.Interfaces;
using System.Threading.Tasks;

namespace Exchange.Client
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var services = new ServiceCollection();
            ConfigureDependencyInjection(services);
            var serviceProvider = services.BuildServiceProvider();
            var cryptopiaService = serviceProvider.GetRequiredService<ICryptopiaService>();
            var bittrexService = serviceProvider.GetRequiredService<IBittrexService>();
            var binanaceService = serviceProvider.GetRequiredService<IBinanceService>();
            var exchangeService = serviceProvider.GetRequiredService<IExchangeServices>();

            while (true)
            {


                Stopwatch sw = new Stopwatch();

                sw.Start();
                Console.WriteLine("Getting Tickers from all exchanges.....");
                var r1 = exchangeService.GetAllExchangeTickersAndPrices();

                Console.WriteLine("Total tickers from all exchanges: {0}ms", r1.Count());
                Console.WriteLine("Total elapsed time for queries: {0}ms", sw.ElapsedMilliseconds);

                sw.Stop();
                sw.Reset();
                sw.Start();


                Console.WriteLine("Eliminating all coins that do not have the potential for arbitrage.....");
                var result = exchangeService.CreateExchangeComparison(r1);
                sw.Stop();
                Console.WriteLine("Total elapsed time to eliminate coins: {0}ms", sw.ElapsedMilliseconds);
                sw.Reset();
                sw.Start();
                Console.WriteLine("Starting arbitrage calculations......");


                var r2 = exchangeService.CalculateArbitrageFromComparison(result);
                sw.Stop();
                Console.WriteLine("Total elapsed time to calculate arbitrage on coins: {0}ms", sw.ElapsedMilliseconds);
                Console.WriteLine("Printing results:");

                foreach (var item in r2.OrderByDescending(c => (((c.Item2.Price - c.Item1.Price) / Math.Abs(c.Item1.Price)) * 100).ToString("N2")))
                {
                    Console.WriteLine(string.Format("PERCENT: {8}\nEXCHANGE: {0}\tSYMBOL: {1}\nEXCHANGE: {4}\tSYMBOL: {5}\nPRICE: {2} VOLUME: {3}\nPRICE: {6} VOLUME: {7}\nASK1: {9}\tASK2: {10}\nBID1: {11}\tBID2: {10}",
                        item.Item1.Exchange,
                        item.Item1.TickerSymbol,
                        item.Item1.Price.ToString("N8"),
                        item.Item1.Volume.ToString("N8"),
                        item.Item2.Exchange,
                        item.Item2.TickerSymbol,
                        item.Item2.Price.ToString("N8"),
                        item.Item2.Volume.ToString("N8"),
                        (((item.Item2.Price - item.Item1.Price) / Math.Abs(item.Item1.Price)) * 100).ToString("N2"),
                        item.Item1.AskPrice.ToString("N8"),
                        item.Item2.AskPrice.ToString("N8"),
                        item.Item1.BidPrice.ToString("N8"),
                        item.Item2.BidPrice.ToString("N8")
                        ));
                }

            }
        }

        public static void ConfigureDependencyInjection(IServiceCollection services)
        {
            services.AddTransient<IExchangeNormalizerService, ExchangeNormalizerService>();
            services.AddTransient<IApiService, ApiService>();
            services.AddTransient<IBinanceService, BinanceService>();
            services.AddTransient<IBittrexService, BittrexService>();
            services.AddTransient<ICryptopiaService, CryptopiaService>();
            services.AddTransient<IExchangeServices, ExchangeServices>();
        }

        public static void Test()
        {


            //Console.ReadLine();
        }
    }
}
