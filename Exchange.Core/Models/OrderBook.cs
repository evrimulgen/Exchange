using System;
using System.Collections.Generic;
using System.Text;

namespace Exchange.Core.Models
{
    public class Order
    {
        public double Price { get; set; }
        public double Volume { get; set; }
    }
    public class OrderBook
    {
        public OrderBook()
        {
            this.Buy = new List<Order>();
            this.Sell = new List<Order>();
        }
        public IEnumerable<Order> Buy { get; set; }
        public IEnumerable<Order> Sell { get; set; }

        public MarketResult Market { get; set; }
    }

    public class ExchangeCoin
    {
        public string Symbol { get; set; }   
        public string Market { get; set; }   
        public string Exchange { get; set; }
        public OrderBook Orders { get; set; }
    }

    public class ExchangeComparison
    {
        public string Symbol { get; set; }
        public string Market { get; set; }
        public ExchangeCoin High { get; set; }
        public ExchangeCoin Low { get; set; }
    }

}
