using Exchange.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Exchange.Core.Interfaces
{
    public interface IExchangeService
    {
        IEnumerable<ICurrencyCoin> ListPrices();
        OrderBook GetMarketOrders(string marketName);
    }
}
