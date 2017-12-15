using Exchange.Cryptopia;
using Exchange.Binance;
using Exchange.Bittrex;

namespace Exchange.Services
{
    public class Foo
    {
        private readonly ICryptopiaService _cryptopiaService;
        private readonly IBittrexService _bittrexService;
        private readonly IBinanceService _binanceService;
        public Foo(
            ICryptopiaService cryptopiaService,
            IBittrexService bittrexService,
            IBinanceService binanceService
        ) 
        {
            _cryptopiaService = cryptopiaService;
            _bittrexService = bittrexService;
            _binanceService = binanceService;
        }

        public void FoFo() 
        {

        }
    }
}