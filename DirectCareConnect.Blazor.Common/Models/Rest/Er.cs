using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.Common.Models.Rest
{
    public class Er
    {
        public int code { get; set; }  // if 0 all is OK
        public string msg { get; set; } // excepion msg or other error
    }
}
