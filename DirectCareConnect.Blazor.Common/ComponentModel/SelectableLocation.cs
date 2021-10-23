using DirectCareConnect.Common.Models.Db.Clients;
using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.Common.ComponentModel
{
    public class SelectableLocation: Location
    {
        public SelectableLocation()
        {

        }

        public SelectableLocation(Location location)
        {
            this.Address = location.Address;
            this.City = location.City;
            this.ClientId = location.ClientId;
            this.ClientLocationId = location.ClientLocationId;
            this.InternalId = location.InternalId;
            this.LastAlert = location.LastAlert;
            this.Latitude = location.Latitude;
            this.LocationTypeId = location.LocationTypeId;
            this.Longitude = location.Longitude;
            this.RadiusInFeet = location.RadiusInFeet;
            this.State = location.State;
            this.Zip = location.Zip;
        }

        public bool Checked { get; set; }
        public bool IsInGeofence { get; set; }
    }
}
