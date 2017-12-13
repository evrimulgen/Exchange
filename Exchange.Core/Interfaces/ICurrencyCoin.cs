using System;
using System.Collections.Generic;
using System.Text;

namespace Exchange.Core.Interfaces
{
    public interface ICurrencyCoin
    {
        string Logo { get; }
        string Exchange { get; }
        string TickerSymbol { get; }
        string Market { get; }
        double Price { get; }
    }
}
