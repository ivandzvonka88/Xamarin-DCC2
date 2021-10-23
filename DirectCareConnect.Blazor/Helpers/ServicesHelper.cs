using DirectCareConnect.Blazor.Components.LoadingOverlay;
using DirectCareConnect.Blazor.Mocks;
using DirectCareConnect.Blazor.Services;
using DirectCareConnect.Common.Impl;
using DirectCareConnect.Common.Impl.Communication;
using DirectCareConnect.Common.Interfaces;
using DirectCareConnect.Common.Interfaces.Authentication;
using DirectCareConnect.Common.Interfaces.Communication;
using DirectCareConnect.Common.Interfaces.Storage;
using DirectCareConnect.Common.Mocks;
using DirectCareConnect.Common.Testing;
using DirectCareConnect.Common.XPlat;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace DirectCareConnect.Blazor.Helpers
{
    public static class ServicesHelper
    {
        [Inject] static IJSRuntime jSRuntime { get; set; }

        private static IModalService _modalService;

        public static void ConfigureCommonServices(IServiceCollection services)
        {
            /*
            services.AddLogging(builder => builder
           .AddBrowserConsole() // Add Blazor.Extensions.Logging.BrowserConsoleLogger
           .SetMinimumLevel(LogLevel.Trace)
           );
           */

            _modalService = new ModalService();
            //Add services shared between multiples project here
            services.AddBlazorMobileNativeServices<Program>();
            //services.AddSingleton<IRestClient, MockedRestClient>();
            
            services.AddSingleton<IModalService>(_modalService);
            services.AddSingleton<ICleanService, CleanService>();
            services.AddSingleton<ILoadingOverlayService, LoadingOverlayService>();
            services.AddSingleton<INotifierService, NotifierService>();

        #if DebugWeb
                services.AddSingleton<IDatabaseService, MockDatabaseService>();
                services.AddSingleton<IMessagingService, MockedMessagingService>();
                services.AddSingleton<ILoginService, LoginService>();
                services.AddSingleton<ILoggingService, LocalLoggingService>();
                services.AddSingleton<IXamarinBridge, MockXamarinBridge>();
                services.AddSingleton<IJobService, MockJobService>();
                
                services.AddSingleton<IRestClient, DddezRestClient>();

    #else
            //services.AddSingleton<IRestClient, DddezRestClient>();
        
#endif





        }

        public static IModalService GetModalService()
        {
            return _modalService;
        }
    }
}
