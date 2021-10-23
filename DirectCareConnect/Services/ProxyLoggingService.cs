using DirectCareConnect.Common.XPlat;
using DirectCareConnect.Services;
using DirectCareConnect.XPlat;
using Microsoft.AppCenter.Analytics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(ProxyLoggingService))]
namespace DirectCareConnect.Services
{
    public class ProxyLoggingService : ILoggingService
    {
        IXLoggingService service;
        public ProxyLoggingService()
        {
            service = DependencyService.Get<IXLoggingService>();
        }
        async public Task LogError(string tag, string message, Dictionary<string, string> json)
        {
            Analytics.TrackEvent($"{tag}:{message}", json);
            Console.WriteLine($"{tag}:{message}");
            await service.LogError(tag, message,json);
            return;
        }

        async public Task LogInfo(string tag, string message, Dictionary<string, string> json)
        {
            Analytics.TrackEvent($"{tag}:{message}", json);
            Console.WriteLine($"{tag}:{message}");
            await service.LogInfo(tag, message, json);
        }

        async public Task LogWarn(string tag, string message, Dictionary<string, string> json)
        {
            Analytics.TrackEvent($"{tag}:{message}", json);
            Console.WriteLine($"{tag}:{message}");
            await service.LogWarn(tag, message, json);
        }
    }
}
