using System.Collections.Generic;
using Exchange.Cryptopia.Model;

namespace Exchange.Cryptopia.APIResults
{
    public class GetMarketsResult : CryptopiaResponse
    {
        public IEnumerable<CryptopiaCoin> Data { get; set; }
    }
}