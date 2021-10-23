using BlazorMobile.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DirectCareConnect.Common.XPlat
{

    [ProxyInterface]
    public interface ILoggingService
    {
        Task LogInfo(string tag, string message, Dictionary<string, string> json=null);
        Task LogWarn(string tag, string message, Dictionary<string, string> json=null);
        Task LogError(string tag, string message, Dictionary<string, string> json=null);
    }
}
