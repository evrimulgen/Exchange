using Exchange.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Exchange.Binance.Model
{
    public class BinanceCoin : ICurrencyCoin
    {
        public string Exchange { get { return "Binance"; } }
        public string Logo { get { return "https://assets.coingecko.com/coins/images/825/small/binance_coin.png?1510040255"; } }
        public string Symbol { get; set; }
        public double Price { get; set; }
        public string Market
        {
            get
            {
                return this.Symbol.Substring(this.Symbol.Length - 3, 3);
            }
        }
        public string TickerSymbol
        {
            get
            {
                return this.Symbol.Substring(0, this.Symbol.Length - 3);
            }
        }
        public double Volume { get; set; }
        public double LastPrice { get; set; }
        public double AskPrice { get; set; }
        public double BidPrice { get; set; }

        public string APIFormatted { get { return string.Format("{0}{1}", TickerSymbol.ToUpper(), Market.ToUpper()); } }
    }
}
