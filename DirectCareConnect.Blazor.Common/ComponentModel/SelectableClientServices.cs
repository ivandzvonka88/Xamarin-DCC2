using DirectCareConnect.Common.Models.Db.Clients;
using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.Common.ComponentModel
{
    public class SelectableClientServices : ClientService
    {
        public SelectableClientServices()
        {

        }
        public SelectableClientServices(ClientService service)
        {
            this.ClientId = service.ClientId;
            this.ClientServiceId = service.ClientServiceId;
            this.ServiceId = service.ServiceId;
            this.ServiceName = service.ServiceName;
            this.ServiceShortName = service.ServiceShortName;
            this.IsHCBS = service.IsHCBS;
            this.IsTherapy = service.IsTherapy;
            this.IsEvaluation = service.IsEvaluation;

        }

        public bool Checked { get; set; }
    }
}
