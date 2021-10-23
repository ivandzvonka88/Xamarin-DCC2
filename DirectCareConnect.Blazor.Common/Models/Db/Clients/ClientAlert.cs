using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.Common.Models.Db.Clients
{
    public class ClientAlert
    {
        [PrimaryKey, AutoIncrement]
        public int AlertId { get; set; }
        public Int64 Priority { get; set; }
        public Int64 ClientId { get; set; }
        public string Alert{ get; set; }
    }
}
