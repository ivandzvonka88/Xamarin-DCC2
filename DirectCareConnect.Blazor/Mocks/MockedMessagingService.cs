using DirectCareConnect.Common.Interfaces.Communication;
using DirectCareConnect.Common.Models.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DirectCareConnect.Blazor.Mocks
{
    public class MockedMessagingService : IMessagingService
    {
        public Task<bool> Connect(CurrentCredentials credentials)
        {
            return Task.FromResult(false);
        }

        public Task Connect()
        {
            throw new NotImplementedException();
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public Task<string> GetToken(string email, string token)
        {
            return Task.FromResult("mockedtoken");
        }

        public void Init()
        {
            throw new NotImplementedException();
        }

        public Task<bool> SendMessage(string json)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SendMessage(string recipient, string message)
        {
            throw new NotImplementedException();
        }

        public void SetToken(string token)
        {
            throw new NotImplementedException();
        }
    }
}
