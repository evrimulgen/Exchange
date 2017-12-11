using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Exchange.Services;
using Exchange.Binance;
using Exchange.Bittrex;
using Exchange.Cryptopia;
using static Exchange.Binance.BinanceClient;
using static Exchange.Bittrex.BittrexClient;
using static Exchange.Cryptopia.CryptopiaClient;

namespace Exchange.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IExchangeNormalizerService, ExchangeNormalizerService>();
            services.AddTransient<IBinanceClient, BinanceClient>();
            services.AddTransient<IBittrexClient, BittrexClient>();
            services.AddTransient<ICryptopiaClient, CryptopiaClient>();
            services.AddTransient<IBinanceService, BinanceService>();
            services.AddTransient<IBittrexService, BittrexService>();
            services.AddTransient<ICryptopiaService, CryptopiaService>();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
