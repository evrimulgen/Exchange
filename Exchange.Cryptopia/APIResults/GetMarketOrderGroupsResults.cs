using Exchange.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Exchange.Cryptopia.APIResults
{
    public class GetMarketOrderGroupsResults : CryptopiaResponse
    {
        public IEnumerable<CryptopiaOrderBook> Data { get; set; }
    }

    public class CryptopiaOrderBook : OrderBook
    {
        public string Market { get; set; }
    }
}
