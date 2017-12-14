using System;
using System.Collections.Generic;
using System.Text;

namespace Exchange.Bittrex.Model
{
    public class BittrexOrderBook
    {
        public BittrexOrderBook()
        {
            this.buy = new List<BittrexOrder>();
            this.sell = new List<BittrexOrder>();
        }

        public IEnumerable<BittrexOrder> buy { get; set; }
        public IEnumerable<BittrexOrder> sell { get; set; }
    }
}
