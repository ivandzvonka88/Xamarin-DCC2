using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.Common.Models.Db
{
    public class ServiceEntry
    {
        [PrimaryKey, AutoIncrement]
        public Int32 ServiceEntryId { get; set; }
        public Int32 CoId { get; set; }

        public Int32 ProviderId { get; set; }

        public Int32 ClientId { get; set; }
        public Int32 ClientServiceId { get; set; }
        public Int32 ServiceId { get; set; }
        public Boolean IsHCBS { get; set; }
        public Boolean IsTherapy{ get; set; }
        public Boolean IsEvaluation { get; set; }
        public String ShortServiceName { get; set; }
        public DateTime StartUTC { get; set; }
        public Decimal StartLat { get; set; }
        public Decimal StartLon { get; set; }
        public Int32? StartClientLocationId { get; set; }
        public Int32 StartLocationTypeId { get; set; }

        public DateTime? EndUTC { get; set; }
        public Decimal EndLat { get; set; }
        public Decimal EndLon { get; set; }

        public int? EndClientLocationId { get; set; }
        public int? EndLocationTypeId { get; set; }
        public Boolean StartAcknowledged { get; set; }
        public Boolean EndAcknowledged { get; set; }
        public Int32 RemoteServiceEntryId { get; set; }

        public  string ToJsonString()
        {
            ServiceEntry jsonObject = this.MemberwiseClone() as ServiceEntry;
            if (!jsonObject.StartClientLocationId.HasValue)
                jsonObject.StartClientLocationId = 0;

            if (!jsonObject.EndClientLocationId.HasValue)
                jsonObject.EndClientLocationId = 0;


            if (!jsonObject.EndLocationTypeId.HasValue)
                jsonObject.EndLocationTypeId = 0;


            return JsonConvert.SerializeObject(jsonObject);
        }

    }
}
