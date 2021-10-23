using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.Common.Impl.Communication.DDezModels
{
    public class SendOpenSessionResponse
    {
        public Er er { get; set; }
        public int deviceSessionId { get; set; }
        public int HCBSEmpHrsId { get; set; }
    }
}
