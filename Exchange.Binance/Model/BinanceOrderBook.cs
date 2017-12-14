using System.Collections.Generic;

namespace Exchange.Binance.Model
{
    public class BinanceOrderBook
    {
        public int lastUpdateId { get; set; }
        public List<List<object>> bids { get; set; }
        public List<List<object>> asks { get; set; }
    }
}
