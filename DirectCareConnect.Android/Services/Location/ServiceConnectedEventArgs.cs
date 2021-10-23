using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace DirectCareConnect.Droid.Services.Location
{
    public class ServiceConnectedEventArgs : EventArgs
    {
        public IBinder Binder { get; set; }
    }
}