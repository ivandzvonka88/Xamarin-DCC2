using DirectCareConnect.Common.Impl.Communication.DDezModels;
using DirectCareConnect.Common.Interfaces.Communication;
using DirectCareConnect.Common.Models.Db;
using DirectCareConnect.Common.Models.Rest;
using DirectCareConnect.Common.Models.UI;
using DirectCareConnect.Common.Models.Users;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;
using DirectCareConnect.Services;
using DirectCareConnect.Common.Models.Global;

[assembly: Xamarin.Forms.Dependency(typeof(ProxyRestClientService))]
namespace DirectCareConnect.Services
{
    public class ProxyRestClientService : IRestClient
    {
        IRestClient clientService;
        public ProxyRestClientService()
        {
            try
            {
                this.clientService = App.Service.GetService<IRestClient>();
            }

            catch             {

            }
        }

        public Task<Result> ConfirmPasswordReset(int userId, string code, string password)
        {
            return this.clientService.ConfirmPasswordReset(userId, code, password);
        }

        public Task<Result> ConfirmPasswordResetCode(int userId, string code)
        {
            return this.clientService.ConfirmPasswordResetCode(userId, code);
        }

        public Task<InitialData> GetInitialData(string token)
        {
            return this.clientService.GetInitialData(token);
        }

        public Task<ClientNote> GetNote(string token, string companyId, string documentId, string docType)
        {
            return this.clientService.GetNote(token, companyId, documentId, docType);
        }

        public Task<List<SchedulerEvent>> GetSchedule(string token, string providerId, string companyId, DateTime start, DateTime to)
        {
            return this.clientService.GetSchedule(token, providerId, companyId, start, to);
        }

        public Task<List<User>> GetUsers(string token)
        {
            return this.clientService.GetUsers(token);
        }

        public Task<ForgotPasswordContactModel> RequestPasswordReset(string email)
        {
            return this.clientService.RequestPasswordReset(email);
        }

        public Task<Result> SendPasswordReset(int userId, string contactMethod)
        {
            return this.clientService.SendPasswordReset(userId, contactMethod);
        }

        public Task<bool> SendServiceUpdate(string restToken, ServiceEntry model)
        {
            return this.clientService.SendServiceUpdate(restToken, model);
        }

        public Task<SetNoteResponse> SetNote(string restToken, EndSessionRespiteModel model)
        {
            return this.clientService.SetNote(restToken, model);
        }

        public Task<SetNoteResponse> SetNote(string restToken, EndSessionHabilitationModel model)
        {
            return this.clientService.SetNote(restToken, model);
        }

        public Task<SetNoteResponse> SetNote(string restToken, EndSessionAttendantCareModel model)
        {
            return this.clientService.SetNote(restToken,model);
        }

        public Task<SetNoteResponse> SetNote(string restToken, EndSessionOccupationalTherapyModel model)
        {
            return this.clientService.SetNote(restToken, model);
        }

        public Task<SetNoteResponse> SetNote(string restToken, EndSessionPhysicalTherapyModel model)
        {
            return this.clientService.SetNote(restToken, model);
        }

        public Task<LoginResult> TryLogin(string email, string password)
        {
            return this.clientService.TryLogin(email,password);
        }

        public Task<bool> UpdateWebServer()
        {
            return this.clientService.UpdateWebServer();
        }
    }
}
