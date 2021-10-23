
using DirectCareConnect.Common.Impl;
using DirectCareConnect.Common.Impl.Communication;
using DirectCareConnect.Common.Interfaces;
using DirectCareConnect.Common.Interfaces.Authentication;
using DirectCareConnect.Common.Interfaces.Communication;
using DirectCareConnect.Common.Interfaces.Storage;
using DirectCareConnect.Common.XPlat;
using DirectCareConnect.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace DirectCareConnect
{
    public class Startup
    {
        private static IServiceProvider services;
        public static void ConfigureServices(HostBuilderContext c, IServiceCollection services)
        {
            services.AddSingleton<ILoggingService, ProxyLoggingService>();
            services.AddSingleton<IRestClient, DddezRestClient>();
            services.AddSingleton<IMessagingService, SendBirdMessagingService>();
            services.AddSingleton<IDatabaseService, DatabaseService>();
            services.AddSingleton<ILoginService, LoginService>();
            services.AddSingleton<INotifierService, NotifierService>();
            services.AddSingleton<IJobService, JobService>();
            services.AddLogging(builder => {
                builder.AddConsole();
                builder.AddDebug();
                });
        }

        public static void Init()
        {
            var host = new HostBuilder();
            
            var hostBuild=host.ConfigureHostConfiguration(c =>
            {
                // Tell the host configuration where to file the file (this is required for Xamarin apps)
                c.AddCommandLine(new string[] { $"ContentRoot={FileSystem.AppDataDirectory}" });
               
            }).ConfigureServices((c, x) =>
            {
                // Configure our local services and access the host configuration
                ConfigureServices(c, x);
            }).Build();

            //Save our service provider so we can use it later.
            services = hostBuild.Services;
        }

        public static IServiceProvider Service
        {
            get
            {
                return services;
            }
        }
    }
}
