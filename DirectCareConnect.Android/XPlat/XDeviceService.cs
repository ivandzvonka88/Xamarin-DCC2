using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using DirectCareConnect.Droid.XPlat;
using DirectCareConnect.XPlat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

[assembly: Dependency(typeof(XDeviceService))]
namespace DirectCareConnect.Droid.XPlat
{
    public class XDeviceService : IXDeviceService
    {
        public string VersionName
        {
            get
            {
                return Android.App.Application.Context.ApplicationContext.PackageManager.GetPackageInfo(Android.App.Application.Context.ApplicationContext.PackageName, 0).VersionName;
            }
        }

        public int VersionCode
        {
            get
            {
                var t = Android.OS.Build.VERSION.SdkInt;
                return (int)t;
            }
        }

        public string AppVersionName
        {
            get
            {
                return Android.App.Application.Context.ApplicationContext.PackageManager.GetPackageInfo(Android.App.Application.Context.ApplicationContext.PackageName, 0).VersionName;
            }
        }

        public long AppVersionCode
        {
            get
            {
                var t = Android.App.Application.Context.ApplicationContext.PackageManager.GetPackageInfo(Android.App.Application.Context.ApplicationContext.PackageName, 0);
                return t.LongVersionCode;
            }
        }

    }
}