using DirectCareConnect.Common.Models.Rest;
using DirectCareConnect.Common.Models.UI;
using DirectCareConnect.Common.Models.Users;
using DirectCareConnect.Common.Models.Db;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DirectCareConnect.Common.Impl.Communication.DDezModels;
using BlazorMobile.Common.Attributes;
using DirectCareConnect.Common.Models.Global;

namespace DirectCareConnect.Common.Interfaces.Communication
{
    /// <summary>
    /// Should not be called directly from Blazor
    /// </summary>
    [ProxyInterface]
    public interface IRestClient
    {
        Task<LoginResult> TryLogin(string email, string password);
        Task<List<User>> GetUsers(string token);
        Task<InitialData> GetInitialData(string token);

        Task<ClientNote> GetNote(string token,string companyId, string documentId, string docType);
        Task<SetNoteResponse> SetNote(string restToken, EndSessionRespiteModel model);
        Task<SetNoteResponse> SetNote(string restToken, EndSessionHabilitationModel model);
        Task<SetNoteResponse> SetNote(string restToken, EndSessionAttendantCareModel model);
        Task<SetNoteResponse> SetNote(string restToken, EndSessionOccupationalTherapyModel model);
        Task<SetNoteResponse> SetNote(string restToken, EndSessionPhysicalTherapyModel model);
        Task<bool> SendServiceUpdate(string restToken, ServiceEntry model);
        Task<List<SchedulerEvent>> GetSchedule(string token, string providerId, string companyId, DateTime start, DateTime to);

        Task<bool> UpdateWebServer();

        #region PASSWORD_RESET
        Task<ForgotPasswordContactModel> RequestPasswordReset(string email);
        Task<Result> SendPasswordReset(int userId, string contactMethod);

        Task<Result> ConfirmPasswordResetCode(int userId, string code);
        Task<Result> ConfirmPasswordReset(int userId, string code, string password);
        #endregion
    }
}


