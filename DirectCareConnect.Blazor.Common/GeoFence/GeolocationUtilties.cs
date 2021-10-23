using DirectCareConnect.Common.Handlers;
using DirectCareConnect.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.Common.GeoFence
{
    public enum DistanceType { Miles, Kilometers };
    /// <summary>  
    /// Specifies a Latitude / Longitude point.  
    /// </summary>  
    public struct Position
    {
        public double Latitude;
        public double Longitude;
    }

    public class GeolocationUtilties
    {
        public double Distance(Position pos1, Position pos2, DistanceType type)
        {
            double R = (type == DistanceType.Miles) ? 3960 : 6371;
            double dLat = this.toRadian(pos2.Latitude - pos1.Latitude);
            double dLon = this.toRadian(pos2.Longitude - pos1.Longitude);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(this.toRadian(pos1.Latitude)) * Math.Cos(this.toRadian(pos2.Latitude)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));
            double d = R * c;
            return d;
        }
        /// <summary>  
        /// Convert to Radians.  
        /// </summary>  
        /// <param name="val"></param>  
        /// <returns></returns>  
        private double toRadian(double val)
        {
            return (Math.PI / 180) * val;
        }
    }

}
