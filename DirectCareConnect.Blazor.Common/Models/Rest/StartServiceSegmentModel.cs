using DirectCareConnect.Common.Models.Db.Clients;
using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.Common.Models.Rest
{
    public class StartServiceSegmentModel
    {
        public int ClientId { get; set; }
        public int ClientServiceId { get; set; }
        public  int ServiceId { get; set; }
        public Location Location { get; set; }
        public  bool IsHCBS { get; set; }
        public  bool IsTherapy { get; set; }
        public bool IsEvaluation { get; set; }
        public string ShortServiceName { get; set; }
    }
}
