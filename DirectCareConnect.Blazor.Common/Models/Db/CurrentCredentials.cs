using SQLite;
using System;

namespace DirectCareConnect.Common.Models.Db
{
    public class CurrentCredentials
    {
        [PrimaryKey, AutoIncrement]
        public Int32 DBId { get; set; }
        public string Credentials { get; set; }
        public DateTime LastRefreshed { get; set; }
        public string Token { get; set; }
        public DateTime TokenIssued { get; set; }
        public string MessagingToken { get; set; }
        public string Email { get; set; }
    }
}
