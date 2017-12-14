using Exchange.Bittrex.Model;
using System.Collections.Generic;

namespace Exchange.Bittrex.APIResults
{
    public class GetMarketSummariesResult : BittrexResponse
    {
        public List<BittrexCoin> result { get; set; }
    }

}
