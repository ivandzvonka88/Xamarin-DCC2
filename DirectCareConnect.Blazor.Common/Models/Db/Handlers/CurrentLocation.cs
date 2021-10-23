using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.Common.Models.Db.Handlers
{
    public class CurrentLocation
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }
        public float Speed { get; set; }
        public float Accuracy { get; set; }
        public float Bearing { get; set; }
        public bool Mocked{ get; set; }

    }
}
