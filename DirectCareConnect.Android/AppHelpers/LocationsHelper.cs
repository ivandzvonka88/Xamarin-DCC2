using Android.Util;
using DirectCareConnect.Common.GeoFence;
using Android.Locations;

namespace DirectCareConnect.Droid.AppHelpers
{
    /// <summary>
    /// Almost 100% sure this entire class can be removed
    /// </summary>
    public class LocationsHelper
    {
        static readonly string TAG = "LocationsHelper";
        private MainActivity activity;
        public LocationsHelper(MainActivity activity)
        {
            this.activity = activity;
        }
        public void SetupLocations()
        {
            App.Current.LocationServiceConnected += (sender, e) =>
            {
                Log.Debug(TAG, "ServiceConnected Event Raised");
                // notifies us of location changes from the system
                App.Current.LocationService.LocationChanged += HandleLocationChanged;
                //notifies us of user changes to the location provider (ie the user disables or enables GPS)
                App.Current.LocationService.ProviderDisabled += HandleProviderDisabled;
                App.Current.LocationService.ProviderEnabled += HandleProviderEnabled;
                // notifies us of the changing status of a provider (ie GPS no longer available)
                App.Current.LocationService.StatusChanged += HandleStatusChanged;
            };
        }
        public void HandleLocationChanged(object sender, Android.Locations.LocationChangedEventArgs e)
        {
            var location = e.Location;


            GeolocationUtilties util = new GeolocationUtilties();
            var miles = util.Distance(new Position
            {
                Latitude = location.Latitude,
                Longitude = location.Longitude
            },

            new Position
            {
                Latitude = 40.8703424,
                Longitude = -74.3716515
            }, DistanceType.Miles
            );

            // these events are on a background thread, need to update on the UI thread
            this.activity.RunOnUiThread(() =>
            {

                Log.Debug(TAG, "Foreground updating miles" + miles);
                var latText = $"Latitude: {location.Latitude}";
                var longText = $"Longitude: {location.Longitude}";
                var altText = $"Altitude: {location.Altitude}";
                var speedText = $"Speed: {location.Speed}";
                var accText = $"Accuracy: {location.Accuracy}";
                var bearText = $"Bearing: {location.Bearing}";
            });
        }

        private void HandleProviderDisabled(object sender, ProviderDisabledEventArgs e)
        {
            Log.Debug(TAG, "Location provider disabled event raised");
        }

        private void HandleProviderEnabled(object sender, ProviderEnabledEventArgs e)
        {
            Log.Debug(TAG, "Location provider enabled event raised");
        }

        private void HandleStatusChanged(object sender, StatusChangedEventArgs e)
        {
            Log.Debug(TAG, "Location status changed, event raised");
        }
    }
}