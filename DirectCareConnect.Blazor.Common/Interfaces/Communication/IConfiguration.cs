using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.Common.Interfaces.Communication
{
    public interface IConfiguration
    {
        string ApiBaseAddress { get; set; }
        string SendBirdAppId { get; set; }
        bool MockedRest { get; set; }

        void Init();
    }
}
