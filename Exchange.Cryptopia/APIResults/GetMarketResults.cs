using Exchange.Cryptopia.Model;

namespace Exchange.Cryptopia.APIResults
{
    public class GetMarketResults : CryptopiaResponse
    {
        public CryptopiaCoin Data { get; set; }
    }
}