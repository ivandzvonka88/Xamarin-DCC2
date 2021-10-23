 using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.Common.Models.Db
{
    public class Service
    {
        [PrimaryKey]
        public Int32 ServiceId { get; set; }
        public string ServiceName { get; set; }
    }
}
