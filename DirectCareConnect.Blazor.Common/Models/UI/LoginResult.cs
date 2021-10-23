using DirectCareConnect.Common.Models.Global;
using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.Common.Models.UI
{
    public class LoginResult:Result
    {
        public LoginResult() : base()
        {

        }

        public LoginResult(bool success) : base(success) { 
        }
        public string EmailAddress { get; set; }
        public string Token { get; set; }

    }
}
