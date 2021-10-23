using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using DirectCareConnect;
using BlazorMobile.Droid.Services;
using Android.Support.V7.App;
using Android.Gms.Common;
using Firebase.Iid;
using Android.Gms.Tasks;
using Firebase;
using Android.Content;
using Android.Util;
using Android;
using DirectCareConnect.Common.GeoFence;
using Android.Locations;
using Android.Support.V4.Content;
using Android.Support.V4.App;
using Android.Support.Design.Widget;
using DirectCareConnect.Droid.AppHelpers;
using BlazorMobile.Droid.Platform;
using Xamarin.Forms;
using BlazorMobile.Services;
using DirectCareConnect.Services;
using DirectCareConnect.Droid.Services;
using DirectCareConnect.AppPackageProject;
using System.Net;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace DirectCareConnect.Droid
{
    [Activity(Label = "Direct Care Connect", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, LaunchMode =LaunchMode.SingleTop)]
    public class MainActivity : BlazorMobileFormsAppCompatActivity, IOnCompleteListener
    {
        
        

        static readonly int RC_REQUEST_LOCATION_PERMISSION = 1000;
        static readonly string TAG = "MainActivity";
        static readonly string[] REQUIRED_PERMISSIONS = { Manifest.Permission.AccessFineLocation, Manifest.Permission.Camera, Manifest.Permission.ReadExternalStorage, Manifest.Permission.WriteExternalStorage, Manifest.Permission.WakeLock, Manifest.Permission.ManageDocuments };
        public static Context AppContext;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            Android.Webkit.WebView.SetWebContentsDebuggingEnabled(true);
            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            BlazorWebViewService.Init(this);
            
            
            SetupPermissions();
            DependencyService.Register<IAssemblyService, AssemblyService>();

            //Register our Blazor app package
            WebApplicationFactory.RegisterAppStreamResolver(AppPackageHelper.ResolveAppPackageStream);

			LoadApplication(new DirectCareConnect.App());
            NotificationsHelper.SetupNotifications(this);
        }

        private void IterateViewGroup(ViewGroup view)
        {
            if (view == null)
                return;

            
            int cnt = view.ChildCount;
            for (int x = 0; x < cnt; x++)
            {
                var vw = view.GetChildAt(x);
                IterateViewGroup(vw as ViewGroup);
            }
        }

        protected override void OnNewIntent(Intent intent)
        {
            try
            {
                if (Intent.Extras != null)
                {
                    foreach (var key in Intent.Extras.KeySet())
                    {
                        var value = Intent.Extras.GetString(key);
                        Log.Debug("Extra", "Key: {0} Value: {1}", key, value);
                    }
                }
            }

            catch { }
            base.OnNewIntent(intent);

        }


        public void OnComplete(Task task)
        {
            
        }


        private void SetupPermissions()
        {
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) == (int)Permission.Granted)
            {
                Log.Debug(TAG, "User already has granted permission.");
                App.StartLocationService();
            }
            else
            {
                Log.Debug(TAG, "Have to request permission from the user. ");
                if (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.AccessFineLocation))
                {
                    var layout = FindViewById(Android.Resource.Id.Content);
                    Snackbar.Make(layout,
                                  Resource.String.permission_location_rationale,
                                  Snackbar.LengthIndefinite)
                            .SetAction(Resource.String.ok,
                                       new Action<Android.Views.View>(delegate
                                       {
                                           ActivityCompat.RequestPermissions(this, REQUIRED_PERMISSIONS,
                                                                         RC_REQUEST_LOCATION_PERMISSION);
                                       })
                                      ).Show();
                }
                else
                {
                    ActivityCompat.RequestPermissions(this, REQUIRED_PERMISSIONS, RC_REQUEST_LOCATION_PERMISSION);
                }
            }


            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.Camera) != Permission.Granted || ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) != Permission.Granted || ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) != Permission.Granted || ContextCompat.CheckSelfPermission(this, Manifest.Permission.WakeLock) != Permission.Granted)
                //ask for authorisation
                ActivityCompat.RequestPermissions((Activity)this, new String[] { Manifest.Permission.Camera, Manifest.Permission.ReadExternalStorage, Manifest.Permission.WriteExternalStorage, Manifest.Permission.WakeLock }, 50);

        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            foreach(var permission in permissions)
            {
                if(permission == Manifest.Permission.AccessFineLocation)
                {
                    App.StartLocationService();
                }
            }
            
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

