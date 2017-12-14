namespace Exchange.Binance.Model
{
    public class BinanceMarketResult
    {
        public string priceChange { get; set; }
        public string priceChangePercent { get; set; }
        public string weightedAvgPrice { get; set; }
        public string prevClosePrice { get; set; }
        public double lastPrice { get; set; }
        public string bidPrice { get; set; }
        public string askPrice { get; set; }
        public string openPrice { get; set; }
        public string highPrice { get; set; }
        public string lowPrice { get; set; }
        public double volume { get; set; }
        public long openTime { get; set; }
        public long closeTime { get; set; }
        public int fristId { get; set; }
        public int lastId { get; set; }
        public int count { get; set; }
    }
}
