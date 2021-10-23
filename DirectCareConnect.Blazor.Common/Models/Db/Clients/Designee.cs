using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.Common.Models.Db.Clients
{
    public partial class Designee
    {
        public string UniqueDesigneeId { get; set; }

        public int ClientId{ get; set; }
        public int DCCDesigneeId { get; set; }
        public int DCCGuardianId { get; set; }

        public bool IsPrimaryGuardian { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Pin { get; set; }
    }
}
