using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreLocation;
using Foundation;
using UIKit;

namespace DirectCareConnect.iOS.Location
{
    public class LocationUpdatedEventArgs : EventArgs
    {
        CLLocation location;

        public LocationUpdatedEventArgs(CLLocation location)
        {
            this.location = location;
        }

        public CLLocation Location
        {
            get { return location; }
        }
    }
}