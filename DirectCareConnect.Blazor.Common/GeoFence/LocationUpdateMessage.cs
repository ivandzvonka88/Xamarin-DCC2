using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.Common.GeoFence
{
    public class LocationUpdateMessage
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public double Speed { get; set; }

        public double Bearing { get; set; }

        public double Accuracy { get; set; }

    }
}
