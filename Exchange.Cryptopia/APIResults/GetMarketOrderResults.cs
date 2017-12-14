using Exchange.Core.Models;

namespace Exchange.Cryptopia.APIResults
{
    public class GetMarketOrderResults : CryptopiaResponse
    {
        public OrderBook Data { get; set; }
    }
}