using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.Common.Models.Db.Clients
{
    public class ClientService
    {
        
        public Int32 ClientId { get; set; }

        [PrimaryKey]
        public Int32 ClientServiceId { get; set; }

        public Int32 ServiceId { get; set; }      

        public string ServiceName { get; set; }

        public string ServiceShortName { get; set; }

        public string NoteType { get; set; }

        public bool IsHCBS { get; set; }

        public bool IsTherapy { get; set; }

        public bool IsEvaluation { get; set; }

        public string CleanNote { get; set; }

        public double WeeklyHours { get; set; }
    }
}
