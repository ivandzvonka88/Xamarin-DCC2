using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.Common.Models.Db.Clients
{
    public class PendingDocumentation
    {
        [PrimaryKey, AutoIncrement]
        public int PendingDocumentationId { get; set; }
        public Int64 ClientServiceId { get; set; }
        public Int64 ClientId { get; set; }
        public Int64 DocumentId { get; set; }
        public Int64 InternalDocumentId { get; set; }
        public string Alert { get; set; }
        public string Svc { get; set; }
    }
}
