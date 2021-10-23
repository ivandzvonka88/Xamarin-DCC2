using DirectCareConnect.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DirectCareConnect.Common.Impl
{
    public class BaseService: IBaseService
    {
        private string message;
        async public Task<bool> Message(string message)
        {
            this.message = message;
            return await Task.FromResult(true);
        }

        async  public Task<string> Message()
        {
            return await Task.FromResult(this.message);
        }
    }

}
