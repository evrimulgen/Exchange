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
    }

}
