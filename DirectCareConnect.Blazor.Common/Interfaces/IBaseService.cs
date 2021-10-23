using BlazorMobile.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DirectCareConnect.Common.Interfaces
{
    [ProxyInterface]
    public interface IBaseService
    {
        Task<bool> Message(string message);
        Task<string> Message();
    }
}
