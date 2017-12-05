using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Exchange.Core.Interfaces
{
    public interface IPublicExchangeClient
    {
        Task<T> GetAsync<T>(string endpoint, string args = null);
    }
}
