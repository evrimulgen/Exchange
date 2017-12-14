using Exchange.Services;
using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Exchange.Cryptopia;
using Exchange.Binance;
using static Exchange.Binance.BinanceClient;
using Exchange.Bittrex;
using static Exchange.Bittrex.BittrexClient;
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

            var results = cryptopiaService.ListPrices().Result;

            Console.WriteLine("Found {0} sybmols!", results.Count());

        }

        public static void ConfigureDependencyInjection(IServiceCollection services)
        {
            services.AddTransient<IExchangeNormalizerService, ExchangeNormalizerService>();
            services.AddTransient<IBinanceClient, BinanceClient>();
            services.AddTransient<IBittrexClient, BittrexClient>();
            services.AddTransient<IApiService, ApiService>();
            services.AddTransient<IBinanceService, BinanceService>();
            services.AddTransient<IBittrexService, BittrexService>();
            services.AddTransient<ICryptopiaService, CryptopiaService>();
        }
    }
}
