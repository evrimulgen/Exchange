using Exchange.Bittrex.Model;
using Exchange.Bittrex.APIResults;

namespace Exchange.Bittrex.APIResults
{
    public class GetTickerResults : BittrexResponse
    {
        public BittrexTicker result { get; set; }
    }
}