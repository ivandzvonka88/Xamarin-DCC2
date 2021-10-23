using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.Common.Impl.Communication.DDezModels
{
    public class Company
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("coId")]
        public string CompanyId { get; set; }

        [JsonProperty("providerId")]
        public string ProviderId { get; set; }

        [JsonProperty("clients")]
        public List<Client> Clients { get; set; }

        [JsonProperty("clientAlerts")]
        public List<ClientAlert> ClientAlerts { get; set; }

        [JsonProperty("credentials")]
        public List<Credential> Credentials { get; set; }

        [JsonProperty("pendingDocumentation")]
        public List<PendingDocumentation> PendingDocumentation { get; set; }

    }
}
