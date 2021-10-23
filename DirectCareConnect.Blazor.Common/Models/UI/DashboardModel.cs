using DirectCareConnect.Common.Models.Db.Clients;
using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.Common.Models.UI
{
    public class DashboardModel
    {
        public DashboardModel()
        {

        }
        public List<ClientAlert> ClientAlerts{get;set;}
        public List<Credential> Credentials { get; set; }
        public List<PendingDocumentation> PendingDocumentations { get; set; }
        public double? SessionTimeInSeconds { get; set; }
        public double? WeeklyAvgAllowedInMinutes { get; set; }

        public bool ShowHeader { get; set; }
        public bool ShowDashboard { get; set; }
        public bool ShowClientAlerts { get; set; }
        public bool ShowCredentials { get; set; }
        public bool ShowDocumentation { get; set; }



    }
}
