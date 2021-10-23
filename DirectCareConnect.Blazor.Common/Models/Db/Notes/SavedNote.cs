using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.Common.Models.Db.Notes
{
    public class SavedNote
    {
        [PrimaryKey, AutoIncrement]
        public Int32 NoteId { get; set; }

        public int ClientId { get; set; }
        public int ServiceId { get; set; }
        public int InternalServiceEntryId { get; set; }
        public int ExternalServiceEntryId { get; set; }
        public string JsonNote{ get; set; }
    }
}
