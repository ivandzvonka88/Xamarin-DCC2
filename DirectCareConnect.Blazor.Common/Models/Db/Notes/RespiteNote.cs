using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.Common.Models.Db.Notes
{
    public class RespiteNote
    {
        [PrimaryKey, AutoIncrement]
        public Int32 RespiteNoteId { get; set; }
        public Int32 ClientId { get; set; }
        public Int32 ServiceId { get; set; }
        public DateTime NoteUtc { get; set; }
        public bool NoShow { get; set; }
        public string Note { get; set; }
        public string PathToUpload { get; set; }
        public string OriginalFileName { get; set; }
    }
}
