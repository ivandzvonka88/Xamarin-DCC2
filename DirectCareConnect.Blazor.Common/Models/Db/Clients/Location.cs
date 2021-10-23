using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.Common.Models.Db.Clients
{
    public class Location
    {
        [PrimaryKey, AutoIncrement]
        public int InternalId { get; set; }
        public int ClientId { get; set; }

        public int ClientLocationId { get; set; }
        public int LocationTypeId{ get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public long RadiusInFeet { get; set; }
        public DateTime? LastAlert{ get; set; }

        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

        [Ignore]
        public double RadiusInMiles
        {
            get
            {
                return this.RadiusInFeet * .000189394;
            }
        }

    }
}
