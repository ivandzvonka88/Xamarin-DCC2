using DirectCareConnect.Common.Models.Db;
using DirectCareConnect.Common.Models.Db.Clients;
using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.Common.Models.Rest
{
    public class InitialData
    {
        public List<Client> Clients { get; set; }
        public List<ClientService> ClientServices { get; set; }
        public List<Location> Locations { get; set; }
        public List<Service> Services { get; set; }
        public List<ClientAlert> ClientAlerts { get; set; }
        public List<PendingDocumentation> PendingDocumentation { get; set; }
        public List<Credential> Credentials { get; set; }
        public Company Company { get; set; }
        public List<Designee> Designees { get; set; }
        public List<ClientNote> ClientNotes { get; set; }
    }
}
