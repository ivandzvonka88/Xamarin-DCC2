using DirectCareConnect.Common.Impl.Communication.DDezModels;
using DirectCareConnect.Common.Interfaces.Communication;
using DirectCareConnect.Common.Models.Rest;
using DirectCareConnect.Common.Models.UI;
using DirectCareConnect.Common.Models.Users;
using DirectCareConnect.Common.Models.Db;

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DirectCareConnect.Common.Models.Global;

namespace DirectCareConnect.Common.Impl.Communication
{
    public class MockedRestClient : IRestClient
    {
        private List<User> mockedUsers;
        private InitialData initialData;
        public MockedRestClient()
        {
            var a = this.GetType().Assembly;
            using (Stream stream = a.GetManifestResourceStream("DirectCareConnect.Common.Resources.MockedData.json"))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    var json = reader.ReadToEnd();
                    MockedJson mocked = JsonConvert.DeserializeObject<MockedJson>(json);
                    this.mockedUsers = mocked.Users;
                    using (Stream stream2 = a.GetManifestResourceStream("DirectCareConnect.Common.Resources.InitialDataFromRest.json"))
                    {
                        using (StreamReader reader2 = new StreamReader(stream2))
                        {
                            var json2 = reader2.ReadToEnd();
                            this.initialData = ProviderConfiguration.FromJson(json2).ToInitialData();
                        }

                    }
                }

            }
        }

        async public Task<InitialData> GetInitialData(string token)
        {
            return await Task.FromResult(this.initialData);
        }

        public Task<ClientNote> GetNote(string token, string companyId, string documentId, string docType)
        {
            throw new NotImplementedException();
        }

        async public Task<List<User>> GetUsers(string token)
        {
            return await Task.FromResult(this.mockedUsers);
        }

        async public Task<Communication.DDezModels.SetNoteResponse> SetNote(string restToken,  EndSessionRespiteModel model)
        {
            await Task.Yield();
            throw new NotImplementedException();
            
        }

        async public Task<Communication.DDezModels.SetNoteResponse> SetNote(string restToken, EndSessionHabilitationModel model)
        {
            await Task.Yield();
            throw new NotImplementedException();
        }

        async public Task<Communication.DDezModels.SetNoteResponse> SetNote(string restToken, EndSessionAttendantCareModel model)
        {
            await Task.Yield();
            throw new NotImplementedException();
        }

        async public Task<Communication.DDezModels.SetNoteResponse> SetNote(string restToken, EndSessionOccupationalTherapyModel model)
        {
            await Task.Yield();
            throw new NotImplementedException();
        }

        async public Task<Communication.DDezModels.SetNoteResponse> SetNote(string restToken, EndSessionPhysicalTherapyModel model)
        {
            await Task.Yield();
            throw new NotImplementedException();
        }

        public Task<LoginResult> TryLogin(string email, string password)
        {
            LoginResult status = new LoginResult();

            if (this.mockedUsers.Where(p=>p.Email.ToLower()==email.ToLower() && p.Password==password).Any())
            {
                status.Success = true;
                status.Token = Guid.NewGuid().ToString();
            }
            return Task.FromResult(status);
        }
        async public Task<bool> SendServiceUpdate(string restToken, ServiceEntry model)
        {
            return await Task.FromResult(true);
        }
        async public Task<bool> UpdateWebServer()
        {
            return await Task.FromResult(true);
        }

        async public Task<List<SchedulerEvent>> GetSchedule(string token, string providerId, string companyId, DateTime start, DateTime to)
        {
            await Task.Yield();
            throw new NotImplementedException();
        }

        public Task<ForgotPasswordContactModel> RequestPasswordReset(string email)
        {
            throw new NotImplementedException();
        }

        public Task<Result> SendPasswordReset(int userId, string contactMethod)
        {
            throw new NotImplementedException();
        }

        public Task<Result> ConfirmPasswordResetCode(int userId, string code)
        {
            throw new NotImplementedException();
        }

        public Task<Result> ConfirmPasswordReset(int userId, string code,string password)
        {
            throw new NotImplementedException();
        }

        private class MockedJson
        {
            public List<User> Users { get; set; }
            public ProviderConfiguration InitialData { get; set; }
        }
    }
}
