using System;
using System.Collections.Generic;
using System.Text;

namespace Exchange.Core.Interfaces
{
    public class ArbitrageItem
    {
        public ArbitrageItem()
        {
            this.Prices = new List<ExchangePrice>();
        }
        public string Symbol { get; set; }

        public List<ExchangePrice> Prices { get; set; }
    }

    public class ExchangePrice
    {
        public string Exchange { get; set; }
        public double Price { get; set; }
        public string Volume { get; set; }
    }


    public class ArbitrageViewModel
    {
        public string Symbol { get; set; }
        public double BittrexPrice { get; set; }
        public double BinancePrice { get; set; }
        public dynamic BinanceVolume { get; set; }
        public double CryptopiaPrice { get; set; }

        public double BittrexBinancePercentage
        {
            get
            {
                if (default(double) == BittrexPrice || default(double) == BinancePrice)
                {
                    return default(double);
                }
                else
                {
                    if(BinancePrice > BittrexPrice)
                        return (1 - Math.Abs(BittrexPrice / BinancePrice)) * 100;
                    else
                        return (1 - Math.Abs(BinancePrice / BittrexPrice)) * 100;

                }
            }
        }

        public double BittrexCryptpiaPercentage
        {
            get
            {
                if (default(double) == BittrexPrice || default(double) == CryptopiaPrice)
                {
                    return default(double);
                }
                else
                {
                    if (CryptopiaPrice > BittrexPrice)
                        return (1 - Math.Abs(BittrexPrice / CryptopiaPrice)) * 100;
                    else
                        return (1 - Math.Abs(CryptopiaPrice / BittrexPrice)) * 100;

                }
            }
        }

        public double BinanceCryptopiaPercentage
        {
            get
            {
                if (default(double) == CryptopiaPrice || default(double) == BinancePrice)
                {
                    return default(double);
                }
                else
                {
                    if (CryptopiaPrice > BinancePrice)
                        return (1 -  Math.Abs(BinancePrice / CryptopiaPrice)) * 100;
                    else
                        return (1 - Math.Abs(CryptopiaPrice / BinancePrice)) * 100;
                }
            }
        }
    }
}
