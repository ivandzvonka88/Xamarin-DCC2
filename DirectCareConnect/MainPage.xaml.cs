using BlazorMobile.Components;
using BlazorMobile.Services;
using DirectCareConnect.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Forms.Internals;
using DirectCareConnect.Common.Interfaces.XPlat;
using DirectCareConnect.Handler;
using DirectCareConnect.Common.Interfaces.Communication;
using DirectCareConnect.XPlat;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace DirectCareConnect
{
    [Preserve(AllMembers = true)]
    public partial class MainPage : ContentPage
	{
        IBlazorWebView webview;
        INotifierService notifier;
        bool updateRunning = false;
        bool firstRequestSuccess = false;
        public MainPage()
		{
            InitializeComponent();
            //Blazor WebView agnostic contoller logic

            webview = BlazorWebViewFactory.Create();

            //WebView rendering customization on page
            View webviewView = webview.GetView();
            
            //Manage your native application behavior when an external resource is requested in your Blazor web application
            //Customize your app behavior in BlazorMobile.Sample.Handler.OnBlazorWebViewNavigationHandler.cs file or create your own!
            webview.Navigating += OnBlazorWebViewNavigationHandler.OnBlazorWebViewNavigating;
            webview.Navigated += Webview_Navigated;
            webview.LaunchBlazorApp();
            Xamarin.Forms.Application.Current.On<Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);

            content.Children.Add(webviewView);

            SubscribeToEvents();
            


                Device.StartTimer(TimeSpan.FromSeconds(30), () =>
            {
                
                if (!updateRunning)
                {
                    updateRunning = true;
                    Task.Factory.StartNew(async () =>
                    {
                        bool success=false;
                        // Do the actual request and wait for it to finish.
                        try
                        {
                            var service = App.Service.GetService<IRestClient>();
                            success = await service.UpdateWebServer();
                        }

                        catch
                        {

                        }

                        updateRunning = false;
                        
                    });
                }
                // Don't repeat the timer (we will start a new timer when the request is finished)
                return true;
            });
            
        }

        private void Webview_Navigated(object sender, WebNavigatedEventArgs e)
        {
            if (e.Result != WebNavigationResult.Success)
            {
                if (!firstRequestSuccess)
                {
                    Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                    {
                        webview.Reload();
                        
                        return false;
                    });

                }
            }
            else
            {
                if (!firstRequestSuccess)
                {
                    webview.GetView().VerticalOptions = LayoutOptions.FillAndExpand;
                    webview.GetView().HorizontalOptions = LayoutOptions.FillAndExpand;
                }

                firstRequestSuccess = true;
            }
        }

        private void SubscribeToEvents()
        {
            this.notifier = App.Service.GetService<INotifierService>();
            this.notifier.ModalNotification += Notifier_ModalNotification;
            this.notifier.UpdateNotification += Notifier_UpdateNotification;
        }

        private void Notifier_UpdateNotification(object sender, Common.Handlers.UpdateEventHandler e)
        {
            if (!App.IsInForeground)
            {
                var ms = DependencyService.Get<IXPlatMessage>();
                if (ms != null)
                    ms.SendNotification("Update Alert!!");

            }

            webview.PostMessage<string>("Update", e.UpdateMessage);
        }

        private void Notifier_ModalNotification(object sender, Common.Handlers.ModalEventHandlerEventArgs e)
        {
            if (!App.IsInForeground)
            {
                var ms = DependencyService.Get<IXPlatMessage>();
                if (ms != null)
                    ms.SendNotification("Location Alert!!");

            }
            
            webview.CallJSInvokableMethod("DirectCareConnect.Blazor", "ShowModal", e.ModalTitle, e.ModalName, e.ModalModel);
            //webview.PostMessage<string>("myNotification", "my notification value");
        }

        ~MainPage()
        {
            if (webview != null)
            {
                webview.Navigating -= OnBlazorWebViewNavigationHandler.OnBlazorWebViewNavigating;
            }
        }
    }
}
