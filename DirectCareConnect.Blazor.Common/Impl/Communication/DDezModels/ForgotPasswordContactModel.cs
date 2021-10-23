using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.Common.Impl.Communication.DDezModels
{
    public class ForgotPasswordContactModel
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ContactMethod { get; set; }
        public bool Success { get; set; }
        public string Error { get; set; }
    }
}
