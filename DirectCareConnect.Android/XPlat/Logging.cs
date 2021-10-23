using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using DirectCareConnect.Common.XPlat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using DirectCareConnect.XPlat;
using System.Threading.Tasks;

[assembly: Dependency(typeof(DirectCareConnect.Droid.XPlat.Logging))]
namespace DirectCareConnect.Droid.XPlat
{
    public class Logging : IXLoggingService
    {
        async  public Task LogError(string tag, string message, Dictionary<string, string> json = null)
        {
            Log.Error(tag, message);
            
        }

        async public Task LogInfo(string tag, string message, Dictionary<string, string> json = null)
        {
            Log.Error(tag, message);
        }

        async  public Task LogWarn(string tag, string message, Dictionary<string, string> json = null)
        {
            Log.Error(tag, message);
        }
    }
} 