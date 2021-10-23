using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.CloudMessaging;
using Foundation;
using Newtonsoft.Json;
using UIKit;
using UserNotifications;

namespace DirectCareConnect.iOS.Communication
{
    public class UserNotificationCenterDelegate : UNUserNotificationCenterDelegate, IMessagingDelegate, IUNUserNotificationCenterDelegate
    {
        public UserNotificationCenterDelegate() { }

        [Export("userNotificationCenter:willPresentNotification:withCompletionHandler:")]
        public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            completionHandler(UNNotificationPresentationOptions.Alert);
        }

        public override void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
        {
            var ccid = response.Notification.Request.Content.UserInfo["cometChatId"].ToString();
            var requestType = response.Notification.Request.Content.UserInfo["requestType"].ToString();

            switch (requestType)
            {
                case "chat":
                    var chatId = response.Notification.Request.Content.UserInfo["chatId"].ToString();
                    //App.NavigateToAcceptChatWindow(ccid, chatId);
                    break;
                case "video":
                    var callId = response.Notification.Request.Content.UserInfo["callId"].ToString();


                    //App.NavigateToAcceptVideoChatWindow(ccid, callId);
                    break;
                case "audio":



                    //App.NavigateToAcceptAudioChatWindow(ccid);
                    break;
            }
            completionHandler();
        }
        // iOS 10, fire when recieve notification foreground


        [Export("messaging:didReceiveMessage:")]
        async public void DidReceiveMessage(Firebase.CloudMessaging.Messaging messaging, Firebase.CloudMessaging.RemoteMessage remoteMessage)
        {
            await Task.Yield();

        }

        public void DidRefreshRegistrationToken(Messaging messaging, string fcmToken)
        {
            // throw new NotImplementedException();
        }

        [Export("application:didReceiveRemoteNotification:fetchCompletionHandler:")]
        public void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
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

    }
}