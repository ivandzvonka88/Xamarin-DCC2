using DirectCareConnect.XPlat;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(DirectCareConnect.iOS.XPlat.Logging))]
namespace DirectCareConnect.iOS.XPlat
{
    public class Logging : IXLoggingService
    {
        async public Task LogError(string tag, string message, Dictionary<string, string> json = null)
        {
            await Task.Yield();
            //Log.Error(tag, message);
        }

        async public Task LogInfo(string tag, string message, Dictionary<string, string> json = null)
        {
            await Task.Yield();
            //Log.Error(tag, message);
        }

        async  public Task LogWarn(string tag, string message, Dictionary<string, string> json = null)
        {
            await Task.Yield();
            //Log.Error(tag, message);
        }
    }
}