using Android.Gms.Common;
using Firebase.Iid;
using System;
using Android.App;
using Android.OS;


namespace DirectCareConnect.Droid.AppHelpers
{
    public class NotificationsHelper
    {
        MainActivity activity;
        public static readonly string CHANNEL_ID = "DirectCareConnectChannel";
        public static readonly int NOTIFICATION_ID = 100;
        private NotificationsHelper(MainActivity activity)
        {
            this.activity = activity;
        }
        public static void SetupNotifications(MainActivity activity)
        {
            NotificationsHelper helper = new NotificationsHelper(activity);

            if (helper.IsPlayServicesAvailable())
            {
                helper.CreateNotificationChannel();
            }

            var log = FirebaseInstanceId.Instance.GetInstanceId().AddOnCompleteListener(activity);
        }

        private bool IsPlayServicesAvailable()
        {
            string msg;
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this.activity);
            if (resultCode != ConnectionResult.Success)
            {
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                    msg = GoogleApiAvailability.Instance.GetErrorString(resultCode);
                else
                {
                    msg = "This device is not supported";
                    this.activity.Finish();
                }
                return false;
            }
            else
            {
                msg = "Google Play Services is available.";
                return true;
            }
        }

        void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification
                // channel on older versions of Android.
                return;
            }

            var channel = new NotificationChannel(CHANNEL_ID,
                                                  "FCM Notifications",
                                                  NotificationImportance.Default)
            {

                Description = "Firebase Cloud Messages appear in this channel"
            };

            var notificationManager = (NotificationManager)this.activity.GetSystemService(Android.Content.Context.NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }
    }
}