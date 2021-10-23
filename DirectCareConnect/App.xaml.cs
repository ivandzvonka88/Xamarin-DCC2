using BlazorMobile.Common;
using BlazorMobile.Components;
using BlazorMobile.Services;
using DirectCareConnect.Common;
using DirectCareConnect.Common.Interfaces;
using DirectCareConnect.Common.Interfaces.Communication;
using DirectCareConnect.Common.Testing;
using DirectCareConnect.Model;
using Microsoft.JSInterop;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;
using Xamarin.Forms;
using DirectCareConnect.Common.Handlers;

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

using DirectCareConnect.Helpers;
using DirectCareConnect.Services;

namespace DirectCareConnect
{
	public partial class App : BlazorApplication
    {
        public const string BlazorAppPackageName = "DirectCareConnect.Blazor.zip";
        

        static InternalDb database;
        static IServiceProvider services;
        public static App CurrentApp { get; set; }
        public static bool IsInForeground{ get; set; }
    //private static ViewModelLocator _locator;
    //public event EventHandler<LocationChangedEventArgs> StatusChanged = delegate { };

    public App()
        {
            DirectCareConnect.Common.Global.IsXamarin = true;
            InitializeComponent();
            ServiceRegistrationHelper.RegisterServices();

            #if DEBUG
            //This allow remote debugging features
            WebApplicationFactory.EnableDebugFeatures();
            WebApplicationFactory.SetHttpPort(8888);
            #endif

            //Register Blazor application package resolver
            WebApplicationFactory.RegisterAppStreamResolver(() =>
            {
                //This app assembly
                var assembly = typeof(App).Assembly;

                //Name of our current Blazor package in this project, stored as an "Embedded Resource"
                //The file is resolved through AssemblyName.FolderAsNamespace.YourPackageNameFile

                //In this example, the result would be DirectCareConnect.Package.DirectCareConnect.Blazor.zip
                var package = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Package.{BlazorAppPackageName}");
                return package;
            });

            
            DependencyService.Get<IMessagingService>().Init(); 
            Startup.Init();
           
            services = Startup.Service;
            CurrentApp = this;
            MainPage = new MainPage();
            
        }

        public static InternalDb Database
        {
            get
            {
                if (database == null)
                {
                    database = new InternalDb(
                      Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "InternalDb.db3"));
                }
                return database;
            }
        }

        public static IServiceProvider Service
        {
            get
            {
                return services;
            }
        }


        protected override void OnResume()
        {
            IsInForeground = true;
            //commented out to avoid blazor app restarting on resume
            //base.OnResume();
        }

        protected override void OnSleep()
        {
            IsInForeground = false;
            base.OnSleep();
        }

        async protected override void OnStart()
        {
            AppCenter.Start("android=307e1663-f59e-497c-97ef-a021e0eff355;" +
                "ios=49580367-6621-405f-8e54-3d18d7fa0742"
                 // "uwp={Your UWP App secret here};" +

                 ,
                  typeof(Analytics), typeof(Crashes));
            
            AppCenter.LogLevel = LogLevel.Verbose;
            IsInForeground = true;
           

            bool isEnabled = await Analytics.IsEnabledAsync();

            base.OnStart();
        }

    }
}
