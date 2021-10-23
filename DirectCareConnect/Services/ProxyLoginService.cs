using DirectCareConnect.Common.Interfaces.Authentication;
using DirectCareConnect.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Microsoft.Extensions.DependencyInjection;
using DirectCareConnect.Common.Models.Global;
using DirectCareConnect.Common.Models.UI;
using DirectCareConnect.Common.Impl.Communication.DDezModels;

[assembly: Dependency(typeof(ProxyLoginService))]
namespace DirectCareConnect.Services
{
    public class ProxyLoginService : ILoginService
    {
        ILoginService loginService;
        public ProxyLoginService()
        {
            try
            {
                this.loginService = App.Service.GetService<ILoginService>();
            }

            catch
            {

            }
        }

       
        public Task<LoginResult> CheckLogin()
        {
            return this.loginService.CheckLogin();
        }

        public Task<Result> ConfirmPasswordReset(int userId, string code, string password)
        {
            return this.loginService.ConfirmPasswordReset(userId, code, password);
        }

        public Task<Result> ConfirmPasswordResetCode(int userId, string code)
        {
            return this.loginService.ConfirmPasswordResetCode(userId, code);
        }

        public Task<string> GetCurrentToken()
        {
            return this.loginService.GetCurrentToken();
        }

        public Task<LoginResult> Login(string email, string password)
        {
            return this.loginService.Login(email, password);
        }

        public Task LogOut()
        {
            return this.loginService.LogOut();
        }

        public Task<ForgotPasswordContactModel> RequestPasswordReset(string email)
        {
            return this.loginService.RequestPasswordReset(email);
        }

        public Task<Result> SendPasswordReset(int userId, string contactMethod)
        {
            return this.loginService.SendPasswordReset(userId, contactMethod);
        }
    }
}
