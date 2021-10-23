using DirectCareConnect.Common.Models.Db.Clients;
using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.Common.ComponentModel
{
    public class ClientWithLocation:Client
    {
        public ClientWithLocation()
        {

        }
        public ClientWithLocation(Client client)
        {
            this.ClientId = client.ClientId;
            this.FirstName = client.FirstName;
            this.LastName = client.LastName;
        }

        public int ClientLocationId { get; set; }
        public bool IsInGeofence { get; set; }
        public bool Checked { get; set; }
    }
}
