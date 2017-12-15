namespace Exchange.Binance.Model
{
    public class BinanceMarketResult
    {
        public string priceChange { get; set; }
        public string priceChangePercent { get; set; }
        public string weightedAvgPrice { get; set; }
        public double prevClosePrice { get; set; }
        public double lastPrice { get; set; }
        public double bidPrice { get; set; }
        public double askPrice { get; set; }
        public double openPrice { get; set; }
        public double highPrice { get; set; }
        public double lowPrice { get; set; }
        public double volume { get; set; }
        public long openTime { get; set; }
        public long closeTime { get; set; }
        public int fristId { get; set; }
        public int lastId { get; set; }
        public int count { get; set; }
    }
}
