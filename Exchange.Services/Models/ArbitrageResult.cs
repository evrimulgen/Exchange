using System;
using System.Collections.Generic;
using System.Text;

namespace Exchange.Services.Models
{
    public class ArbitrageResult
    {
        private const string CRYPTOPIA_URL = "https://www.cryptopia.co.nz/Exchange/?market={0}_{1}";
        private const string BITTREX_URL = "https://bittrex.com/Market/Index?MarketName={0}-{1}";
        private const string BINANCE_URL = "https://www.binance.com/tradeDetail.html?symbol={0}_{1}";

        public string Market { get; set; }
        public string Symbol { get; set; }
        public string Exchange1 { get; set; }
        public string Exchange1Logo { get; set; }
        public double Exchange1Price { get; set; }
        public string Exchange2 { get; set; }
        public double Exchange2Price { get; set; }
        public string Exchange2Logo { get; set; }
        public double Percentage
        {
            get
            {
                return ((Exchange2Price - Exchange1Price) / Math.Abs(Exchange1Price)) * 100;
            }
        }

        public string Exchange1Link
        {
            get
            {
                return GetLink(Exchange1);
            }
        }

        public string Exchange2Link
        {
            get
            {
                return GetLink(Exchange2);
            }
        }

        private string GetLink(string exchange)
        {
            string link = string.Empty;
            switch (exchange)
            {
                case "Binance":
                    link = string.Format(BINANCE_URL, this.Symbol, this.Market);
                    break;
                case "Bittrex":
                    link = string.Format(BITTREX_URL, this.Market, this.Symbol);
                    break;
                case "Cryptopia":
                    link = string.Format(CRYPTOPIA_URL, this.Symbol, this.Market);
                    break;
                default:
                    break;
            }
            return link;
        }
    }
}
