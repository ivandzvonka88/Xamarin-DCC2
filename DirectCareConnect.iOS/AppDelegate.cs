using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.CloudMessaging;
using BlazorMobile.iOS.Services;
using DirectCareConnect.Common;
using Foundation;
using UIKit;
using UserNotifications;
using Xamarin.Forms;
using DirectCareConnect.iOS.Communication;
using DirectCareConnect.iOS.Location;
using BlazorMobile.Services;
using DirectCareConnect.Services;
using DirectCareConnect.iOS.Services;
using DirectCareConnect.AppPackageProject;

namespace DirectCareConnect.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, IUNUserNotificationCenterDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        readonly UserNotificationCenterDelegate del = new UserNotificationCenterDelegate();
        public static LocationManager Manager = null;
        public const string InitializedNotificationSettingsKey = "InitializedNotificationSettings";


        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            try
            {
                global::Xamarin.Forms.Forms.Init();
                BlazorWebViewService.Init();

                DependencyService.Register<IAssemblyService, AssemblyService>();

                //Register our Blazor app package
                WebApplicationFactory.RegisterAppStreamResolver(AppPackageHelper.ResolveAppPackageStream);

                if (int.TryParse(UIDevice.CurrentDevice.SystemVersion.Split('.')[0], out int majorVersion) && majorVersion >= 13)
                {
                   // BlazorWebViewService.EnableDelayedStartPatch();
                }
                var xxApp = new App();
                LoadApplication(xxApp);
                Global.IsXamarin = true;
                ConfigureFirebase();
                Manager = new LocationManager();
                Manager.StartLocationUpdates();
                return base.FinishedLaunching(app, options);
            }


            catch
            {
                return false;
            }
        }

        private void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
           
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
           
        }

        private void ConfigureFirebase()
        {
            

            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                UNUserNotificationCenter center = UNUserNotificationCenter.Current;
                var authOptions = UNAuthorizationOptions.ProvidesAppNotificationSettings | UNAuthorizationOptions.Alert | UNAuthorizationOptions.Sound | UNAuthorizationOptions.Provisional | UNAuthorizationOptions.Badge;

                center.RequestAuthorization(authOptions, (granted, error) =>
                {
                    center.Delegate = this;
                    var rotateTwentyDegreesAction = UNNotificationAction.FromIdentifier("rotate-twenty-degrees-action", "Rotate 20°", UNNotificationActionOptions.None);

                    var redCategory = UNNotificationCategory.FromIdentifier(
                        "red-category",
                        new UNNotificationAction[] { rotateTwentyDegreesAction },
                        new string[] { },
                        UNNotificationCategoryOptions.CustomDismissAction
                    );

                    var greenCategory = UNNotificationCategory.FromIdentifier(
                        "green-category",
                        new UNNotificationAction[] { rotateTwentyDegreesAction },
                        new string[] { },
                        UNNotificationCategoryOptions.CustomDismissAction
                    );

                  
                    
                });

                bool initializedNotificationSettings = NSUserDefaults.StandardUserDefaults.BoolForKey(InitializedNotificationSettingsKey);
                if (!initializedNotificationSettings)
                {
                    NSUserDefaults.StandardUserDefaults.SetBool(true, "redNotificationsEnabledKey");
                    NSUserDefaults.StandardUserDefaults.SetBool(true, "greenNotificationsEnabledKey");
                    NSUserDefaults.StandardUserDefaults.SetBool(true, InitializedNotificationSettingsKey);
                }

                UNUserNotificationCenter.Current.Delegate = del;
                Messaging.SharedInstance.Delegate = del;

                // For iOS 10 display notification (sent via APNS)

            }
            else
            {
                // iOS 9 <=
                var allNotificationTypes = UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound;
                var settings = UIUserNotificationSettings.GetSettingsForTypes(allNotificationTypes, null);
                UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
            }


            UIApplication.SharedApplication.RegisterForRemoteNotifications();
            UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound);




            // Firebase component initialize

            Firebase.Core.App.Configure();

            Firebase.InstanceID.InstanceId.Notifications.ObserveTokenRefresh((sender, e) => {
                var newToken = Firebase.InstanceID.InstanceId.SharedInstance.Token;
                // if you want to send notification per user, use this token
                connectFCM();
            });

            var testToken = Firebase.InstanceID.InstanceId.SharedInstance.Token;
        }

        private void connectFCM()
        {

            Messaging.SharedInstance.ShouldEstablishDirectChannel = true;
            Messaging.SharedInstance.Subscribe("/topics/all");
            return;

        }

        public void ApplicationReceivedRemoteMessage(RemoteMessage remoteMessage)
        {




        }
        [Export("userNotificationCenter:didReceiveNotificationResponse:withCompletionHandler:")]
        public void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, System.Action completionHandler)
        {
            if (response.IsDefaultAction)
            {
                
            }
            if (response.IsDismissAction)
            {
                
            }
            else
            {
                
            }

            completionHandler();
        }

        [Export("userNotificationCenter:willPresentNotification:withCompletionHandler:")]
        public void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, System.Action<UNNotificationPresentationOptions> completionHandler)
        {
            completionHandler(UNNotificationPresentationOptions.Alert | UNNotificationPresentationOptions.Sound);
        }

        [Export("userNotificationCenter:openSettingsForNotification:")]
        public void OpenSettings(UNUserNotificationCenter center, UNNotification notification)
        {
           
        }

        [Export("application:didReceiveRemoteNotification:fetchCompletionHandler:")]
        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            var content = new UNMutableNotificationContent();
            content.Title = "Spirilite";
            content.Subtitle = "Message Notification";
            content.Body = "New Chat Audio Request";
            content.Badge = 1;


            content.UserInfo = NSDictionary.FromObjectsAndKeys(new NSString[3]{
                new NSString("One"),
                new NSString("audio"),
                new NSString("Two")
            },
            new NSString[3] {
                new NSString("cometChatId"),
                new NSString("requestType"),
                new NSString("callId")
            });


            var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(5, false);

            var requestID = "sampleRequest";
            var request = UNNotificationRequest.FromIdentifier(requestID, content, trigger);

            UNUserNotificationCenter.Current.AddNotificationRequest(request, (err) =>
            {
                if (err != null)
                {
                    // Do something with error...
                }
            }
            );
            //base.DidReceiveRemoteNotification(application, userInfo, completionHandler);
        }

        

        public override void DidEnterBackground(UIApplication uiApplication)
        {
           
        }

        public override void OnActivated(UIApplication uiApplication)
        {
            connectFCM();
            base.OnActivated(uiApplication);
        }
        public override void DidRegisterUserNotificationSettings(UIApplication application, UIUserNotificationSettings notificationSettings)
        {
            base.DidRegisterUserNotificationSettings(application, notificationSettings);
        }
        

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            //var tokenStringBase64 = deviceToken.GetBase64EncodedString(NSDataBase64EncodingOptions.None);
            //this.xApp.SetFirebaseToken(deviceToken.ToString().Replace("<", "").Replace(">", "").Replace(" ", ""));
            //no need to do anything 
            //base.RegisteredForRemoteNotifications(application, deviceToken);
        }

        public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
        {
            base.ReceivedRemoteNotification(application, userInfo);
        }

        public override void ReceivedLocalNotification(UIApplication application, UILocalNotification notification)
        {
            base.ReceivedLocalNotification(application, notification);
        }


        [Export("messaging:didReceiveMessage:")]
        public void DidReceiveMessage(Firebase.CloudMessaging.Messaging messaging, Firebase.CloudMessaging.RemoteMessage remoteMessage)
        {
            Console.WriteLine(remoteMessage.AppData);
        }
    }
}
