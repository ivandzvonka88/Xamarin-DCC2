using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.Common.Models.ComponentCommunication
{
    public class EndServiceModel
    {
        public int ClientId { get; set; }
        public int ClientServiceId { get; set; }
        public int ServiceId { get; set; }

        public int SessionId { get; set; }
    }
}
