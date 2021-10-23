using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DirectCareConnect.Common.Constants
{
    public enum NoteResolution
    {
        [Description("Designee Will Sign")]
        DesigneeSign,

        [Description("Designee Unable To Sign")]
        DesigneeUnableToSign,
        
        [Description("Designee Refused To Sign,")]
        DesigneeRefusedToSign,

        [Description("Client Refused To Work")]
        ClientRefusedWork,

        [Description("No Show")]
        NoShow,

        [Description("Unsafe To Work")]
        UnsafeToWork
    }
}
