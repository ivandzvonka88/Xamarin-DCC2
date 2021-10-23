using BlazorMobile.Common.Attributes;
using DirectCareConnect.Common.Models.Db;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DirectCareConnect.Common.Interfaces.Communication
{
    [ProxyInterface]
    public interface IMessagingService
    {
        void Init();
        Task<bool> SendMessage(string json);
        Task<bool> SendMessage(string recipient, string message);
        Task<string> GetToken(string email, string token);
        Task<bool> Connect(CurrentCredentials credentials);
        Task Connect();
        void Disconnect();
        void SetToken(string token);
    }
}
