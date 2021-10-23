using Blazored.Toast;
using BlazorMobile.Common;
using BlazorMobile.Common.Services;
using DirectCareConnect.Blazor.Helpers;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DirectCareConnect.Blazor
{
    public class Program
    {
        public static IServiceProvider Services;
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.Services.AddSingleton(new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            #region Services registration

            ServicesHelper.ConfigureCommonServices(builder.Services);
            
            #endregion

            #region DEBUG

            //Only if you want to test WebAssembly with remote debugging from a dev machine
            BlazorMobileService.EnableClientToDeviceRemoteDebugging("127.0.0.1", 8888);

            #endregion


            BlazorMobileService.OnBlazorMobileLoaded += (object source, BlazorMobileOnFinishEventArgs eventArgs) =>
            {
                
            };

            builder.RootComponents.Add<MobileApp>("app");
            builder.Services.AddBlazoredToast();
            await builder.Build().RunAsync();
           // Services = app.Services;
            //CreateHostBuilder(args).Build().Run();
        }

    }
}
