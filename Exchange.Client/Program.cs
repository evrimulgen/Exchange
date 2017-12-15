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
                Console.WriteLine("Strating arbitrage calculations......");
                var r2 = exchangeService.CalculateArbitrageFromComparison(result);
                sw.Stop();
                Console.WriteLine("Total elapsed time to calculate arbitrage on coins: {0}ms", sw.ElapsedMilliseconds);
                Console.WriteLine("Printing results:");
                var i = 1;
                foreach (var item in r2.OrderByDescending(c => (((c.Item2.Price - c.Item1.Price) / Math.Abs(c.Item1.Price)) * 100).ToString("N2")))
                {
                    Console.WriteLine(string.Format("COMPARE: {0},{1},{2},{3},{4},{5},{6},{7}",
                        i++,
                        item.Item1.Exchange,
                        item.Item1.TickerSymbol,
                        item.Item1.Price.ToString("N8"),
                        item.Item2.Exchange,
                        item.Item2.TickerSymbol,
                        item.Item2.Price.ToString("N8"),
                        (((item.Item2.Price - item.Item1.Price) / Math.Abs(item.Item1.Price)) * 100).ToString("N2")
                        ));
                }


                foreach (var market in r2.Where(c => c.Item1.Exchange.Equals("Binance")))
                {
                    var r3 = binanaceService.GetOrderBook(string.Format("{0}{1}", market.Item1.TickerSymbol, market.Item1.Market));
                    Console.WriteLine(string.Format("BINANCE: Checking {0} and found {1} Sell Orders, {2} Buy Orders and ", string.Format("{0}{1}",
                        market.Item1.TickerSymbol, market.Item2.Market), r3.Sell.Count(), r3.Buy.Count()));

                }

                foreach (var market in r2.Where(c => c.Item2.Exchange.Equals("Binance")))
                {
                    var r3 = binanaceService.GetOrderBook(string.Format("{0}{1}", market.Item2.TickerSymbol, market.Item2.Market));
                    Console.WriteLine(string.Format("BINANCE: Checking {0} and found {1} Sell Orders, {2} Buy Orders and ", string.Format("{0}{1}",
                        market.Item2.TickerSymbol, market.Item2.Market), r3.Sell.Count(), r3.Buy.Count()));

                }

                foreach (var market in r2.Where(c => c.Item1.Exchange.Equals("Bittrex")))
                {
                    var r3 = bittrexService.GetOrderBook(string.Format("{1}-{0}", market.Item1.TickerSymbol, market.Item1.Market));
                    Console.WriteLine(string.Format("BITTREX: Checking {0} and found {1} Sell Orders, {2} Buy Orders and ", string.Format("{1}-{0}",
                        market.Item1.TickerSymbol, market.Item2.Market), r3.Sell.Count(), r3.Buy.Count()));

                }

                foreach (var market in r2.Where(c => c.Item2.Exchange.Equals("Bittrex")))
                {
                    var r3 = bittrexService.GetOrderBook(string.Format("{1}-{0}", market.Item2.TickerSymbol, market.Item2.Market));
                    Console.WriteLine(string.Format("BITTREX: Checking {0} and found {1} Sell Orders, {2} Buy Orders and ", string.Format("{1}-{0}",
                        market.Item2.TickerSymbol, market.Item2.Market), r3.Sell.Count(), r3.Buy.Count()));

                }

                foreach (var market in r2.Where(c => c.Item1.Exchange.Equals("Cryptopia")))
                {
                    var r3 = cryptopiaService.GetOrderBook(string.Format("{0}_{1}", market.Item1.TickerSymbol, market.Item1.Market));
                    Console.WriteLine(string.Format("CRYPTOPIA: Checking {0} and found {1} Sell Orders, {2} Buy Orders and ", string.Format("{0}_{1}",
                        market.Item1.TickerSymbol, market.Item2.Market), r3.Sell.Count(), r3.Buy.Count()));

                }

                foreach (var market in r2.Where(c => c.Item2.Exchange.Equals("Cryptopia")))
                {
                    var r3 = cryptopiaService.GetOrderBook(string.Format("{0}_{1}", market.Item2.TickerSymbol, market.Item2.Market));
                    Console.WriteLine(string.Format("CRYPTOPIA: Checking {0} and found {1} Sell Orders, {2} Buy Orders and ", string.Format("{0}_{1}",
                        market.Item2.TickerSymbol, market.Item2.Market), r3.Sell.Count(), r3.Buy.Count()));

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
