using BlazorMobile.Common;
using BlazorMobile.Common.Services;
using DirectCareConnect.Blazor.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System;


namespace DirectCareConnect.Blazor
{
    public class Startup
    {
        public static IServiceProvider Services;
        public void ConfigureServices(IServiceCollection services)
        {
            ServicesHelper.ConfigureCommonServices(services);
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            #region DEBUG

            //Only if you want to test WebAssembly with remote debugging from a dev machine
            BlazorMobileService.EnableClientToDeviceRemoteDebugging("192.168.1.8", 8888);
            

            #endregion

            BlazorMobileService.Init((bool success) =>
            {
               
            });

            app.AddComponent<MobileApp>("app");
            Services = app.Services;
        }
    }
}
