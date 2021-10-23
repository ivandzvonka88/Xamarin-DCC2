using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.Common.Handlers
{
    public class ModalEventHandlerEventArgs: EventArgs
    {
        public string ModalTitle { get; set; }
        public string ModalName { get; set; }
        public object ModalModel { get; set; }
    }
}
