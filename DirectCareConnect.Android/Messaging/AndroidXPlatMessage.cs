using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using DirectCareConnect.Common.Interfaces.XPlat;
using DirectCareConnect.Droid.AppHelpers;
using Xamarin.Forms;

[assembly: Dependency(typeof(DirectCareConnect.Droid.Messaging.AndroidXPlatMessage))]
namespace DirectCareConnect.Droid.Messaging
{
    public class AndroidXPlatMessage : IXPlatMessage
    {
        public void SendNotification(string message)
        {
            var intent = new Intent(Android.App.Application.Context, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            /*
            foreach (var key in data.Keys)
            {
                intent.PutExtra(key, data[key]);
            }
            */
            var pendingIntent = PendingIntent.GetActivity(Android.App.Application.Context,
                                                          NotificationsHelper.NOTIFICATION_ID,
                                                          intent,
                                                          PendingIntentFlags.OneShot);

            var notificationBuilder = new NotificationCompat.Builder(Android.App.Application.Context, NotificationsHelper.CHANNEL_ID)
                                      .SetSmallIcon(Resource.Drawable.ic_audiotrack_light)
                                      .SetContentTitle("FCM Message")
                                      .SetContentText(message)
                                      .SetAutoCancel(true)
                                      .SetContentIntent(pendingIntent);

            var notificationManager = NotificationManagerCompat.From(Android.App.Application.Context);
            notificationManager.Notify(NotificationsHelper.NOTIFICATION_ID, notificationBuilder.Build());
        }
    }
}