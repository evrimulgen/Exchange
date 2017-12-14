using System;
using System.Collections.Generic;
using System.Text;

namespace Exchange.Bittrex.APIResults
{
    public class GetMarketsResult : BittrexResponse
    {
        public IEnumerable<BittrexMarkets> result { get; set; }
    }

    public class BittrexMarkets
    {
        public string MarketCurrency { get; set; }
        public string BaseCurrency { get; set; }
        public string MarketCurrencyLong { get; set; }
        public string BaseCurrencyLong { get; set; }
        public double MinTradeSize { get; set; }
        public string MarketName { get; set; }
        public bool IsActive { get; set; }
        public DateTime Created { get; set; }
    }
}
