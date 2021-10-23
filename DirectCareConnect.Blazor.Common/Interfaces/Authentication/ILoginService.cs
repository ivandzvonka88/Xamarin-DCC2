using BlazorMobile.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DirectCareConnect.Common.Models.Global;
using DirectCareConnect.Common.Models.UI;
using DirectCareConnect.Common.Impl.Communication.DDezModels;

namespace DirectCareConnect.Common.Interfaces.Authentication
{
    [ProxyInterface]
    public interface ILoginService
    {
        Task<LoginResult> Login(string email, string password);
        /// <summary>
        /// Checks to see if locally saved token is still good
        /// </summary>
        /// <returns></returns>
        Task<LoginResult> CheckLogin();

        Task<string> GetCurrentToken();
        Task LogOut();

        #region PASSWORD_RESET
        Task<ForgotPasswordContactModel> RequestPasswordReset(string email);
        Task<Result> SendPasswordReset(int userId, string contactMethod);
        
        Task<Result> ConfirmPasswordResetCode(int userId, string code);
        Task<Result> ConfirmPasswordReset(int userId, string code, string password);
        #endregion
    }
}
