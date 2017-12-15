using Exchange.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Exchange.Bittrex.Model
{
    public class BittrexCoin : BittrexTicker,  ICurrencyCoin
    {
        public string Exchange { get { return "Bittrex"; } }
        public string Logo { get { return "https://bittrex.com/Content/img/logos/bittrex-96.png"; } }
        public string APIFormatted { get { return string.Format("{0}-{1}", TickerSymbol.ToUpper(), Market.ToUpper()); } }
        public string MarketName { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Volume { get; set; }
        public double BaseVolume { get; set; }
        public DateTime TimeStamp { get; set; }
        public int OpenBuyOrders { get; set; }
        public int OpenSellOrders { get; set; }
        public double PrevDay { get; set; }
        public DateTime Created { get; set; }
        public object DisplayMarketName { get; set; }
        public string TickerSymbol
        {
            get
            {
                return this.MarketName.Split('-')[1];
            }
        }

        public string Market
        {
            get
            {
                return this.MarketName.Split('-')[0];
            }
        }

        public double Price
        {
            get
            {
                return this.Last;
            }
        }

        public double LastPrice { get; set; }
        public double AskPrice { get; set; }
        public double BidPrice { get; set; }
    }
}
