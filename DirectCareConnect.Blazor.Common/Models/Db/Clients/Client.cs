using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.Common.Models.Db.Clients
{
    public class Client
    {
        [PrimaryKey]
        public Int32 ClientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
