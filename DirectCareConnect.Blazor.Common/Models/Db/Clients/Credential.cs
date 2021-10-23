using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.Common.Models.Db.Clients
{
    public class Credential
    {
        [PrimaryKey,AutoIncrement]
        public int CredentialId { get; set; }
        public Int64 CoId{get;set;}
        public Int64 ProviderId { get; set; }
        public Int64 CredId { get; set; }
        public Int64 CredTypeId { get; set; }
        public string CredName { get; set; }
        public string DocId { get; set; }
        public string ValidFrom { get; set; }
        public string ValidTo { get; set; }
        public string Status { get; set; }

    }
}
