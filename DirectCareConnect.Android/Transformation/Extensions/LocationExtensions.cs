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
using DirectCareConnect.Common.Models.Db.Handlers;

namespace DirectCareConnect.Droid.Transformation.Extensions
{
    public static class LocationExtensions
    {
        public static CurrentLocation ToCurrentLocation(this Android.Locations.Location location)
        {
            if (location == null)
                return new CurrentLocation();

            return new CurrentLocation
            {
                Accuracy = location.Accuracy,
                Altitude = location.Altitude,
                Bearing = location.Bearing,
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                Speed = location.Speed
            };
        }
    }
}