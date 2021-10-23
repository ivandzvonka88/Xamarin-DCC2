using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.Common.Models.Rest
{
    public class LoggedInStatus
    {
        public bool LoggedIn { get; set; }
        public string Token { get; set; }
        public string ErrorMessage { get; set; }
    }
}
