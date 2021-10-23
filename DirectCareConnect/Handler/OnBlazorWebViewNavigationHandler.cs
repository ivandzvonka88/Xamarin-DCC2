using BlazorMobile.Common;
using BlazorMobile.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Xamarin.Forms;

namespace DirectCareConnect.Handler
{
    public static class OnBlazorWebViewNavigationHandler
    {
        public static void OnBlazorWebViewNavigating(object sender, WebNavigatingEventArgs e)
        {
            e.Cancel = false;
        }
    }
}
