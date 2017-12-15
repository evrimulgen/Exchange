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

            var r1 = cryptopiaService.GetMarketOrderGroupsAync(new string[] { "DOT_BTC", "DOT_LTC", "DOT_DOGE" }).Result;

            var r2 = bittrexService.GetMarketSummariesAsync().Result;


            Console.WriteLine("Found {0} sybmols @ Cryptopia!", r1.Count());
            Console.WriteLine("Found {0} sybmols @ Bittrex!", r2.Count());
            Console.ReadLine();
        }

        public static void ConfigureDependencyInjection(IServiceCollection services)
        {
            services.AddTransient<IExchangeNormalizerService, ExchangeNormalizerService>();
            services.AddTransient<IApiService, ApiService>();
            services.AddTransient<IBinanceService, BinanceService>();
            services.AddTransient<IBittrexService, BittrexService>();
            services.AddTransient<ICryptopiaService, CryptopiaService>();
        }
    }
}
