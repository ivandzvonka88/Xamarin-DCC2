using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.Common.Impl.Communication.DDezModels
{
    public class SchedulerEvent
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("text")]
        public string text { get; set; }
        [JsonProperty("start_date")]
        public DateTime start_date { get; set; }
        [JsonProperty("end_date")]
        public DateTime end_date { get; set; }
        [JsonProperty("rec_type")]
        public string rec_type { get; set; }
        [JsonProperty("event_length")]
        public long event_length { get; set; }
        [JsonProperty("event_pid")]
        public string event_pid { get; set; }
        [JsonProperty("client_fn")]
        public string client_fn { get; set; }
        [JsonProperty("client_ln")]
        public string client_ln { get; set; }
        
        public string ClientFullName { get; set; }

        [JsonProperty("service_name")]
        public string service_name { get; set; }
        [JsonProperty("client_id")]
        public int client_id { get; set; }
        [JsonProperty("isActive")]
        public bool isActive { get; set; }
        
        public string ActionIcons { get; set; }
        
        public string AdditionalInfo { get; set; }
        
        public string ClientPhoneNumber { get; set; }
        
        public string ClientEmail { get; set; }
        
        public string Location { get; set; }

        [JsonProperty("provider_id")]
        public int? provider_id { get; set; }
        [JsonProperty("providerName")]
        public string providerName { get; set; }
        [JsonProperty("service_id")]
        public int? service_id { get; set; }
        [JsonProperty("missedVisit")]
        public bool? missedVisit { get; set; }
        [JsonProperty("resolutionCodeId")]
        public int? resolutionCodeId { get; set; }
        [JsonProperty("reasonCodeId")]
        public int? reasonCodeId { get; set; }
    }
}
