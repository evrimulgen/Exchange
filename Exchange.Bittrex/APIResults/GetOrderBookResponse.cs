using Exchange.Bittrex.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Exchange.Bittrex.APIResults
{
    public class GetOrderBookResponse : BittrexResponse
    {
        public BittrexOrderBook result { get; set; }
    }

}
