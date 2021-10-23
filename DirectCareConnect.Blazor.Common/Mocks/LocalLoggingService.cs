using DirectCareConnect.Common.XPlat;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DirectCareConnect.Common.Mocks
{
    public class LocalLoggingService : ILoggingService
    {
        async public Task LogError(string tag, string message, Dictionary<string, string> json = null)
        {
            await Task.Yield();
            Console.WriteLine($"Error - {tag}: {message}");
        }

        async public Task LogInfo(string tag, string message, Dictionary<string, string> json = null)
        {
            await Task.Yield();
            Console.WriteLine($"Error - {tag}: {message}");
        }

        async public Task LogWarn(string tag, string message, Dictionary<string, string> json = null)
        {
            await Task.Yield();
            Console.WriteLine($"Error - {tag}: {message}");
        }
    }
}
