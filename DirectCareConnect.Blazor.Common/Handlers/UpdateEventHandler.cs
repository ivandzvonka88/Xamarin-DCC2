using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.Common.Handlers
{
    public class UpdateEventHandler: EventArgs
    {
        public string UpdateMessage { get; set; }
    }
}
