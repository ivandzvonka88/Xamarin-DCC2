using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.Common.Interfaces.XPlat
{
    public interface IXPlatMessage
    {
        void SendNotification(string message);
    }
}
