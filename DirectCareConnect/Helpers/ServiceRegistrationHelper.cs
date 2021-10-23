using DirectCareConnect.Common.Interfaces;
using DirectCareConnect.Common.XPlat;
using DirectCareConnect.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.Helpers
{
    public static class ServiceRegistrationHelper
    {
        /// <summary>
        /// Register the Xamarin application services.
        /// Calling  DependencyService.Register is the recommended way to register services,
        /// as some platforms like UWP tend to strip service registration if declared from a namespace attribute
        /// </summary>
        public static void RegisterServices()
        {
            Xamarin.Forms.DependencyService.Register<IXamarinBridge, XamarinBridge>();
            Xamarin.Forms.DependencyService.Register<ILoggingService, ProxyLoggingService>();

        }
    }
}
