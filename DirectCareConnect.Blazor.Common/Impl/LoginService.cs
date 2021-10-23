using DirectCareConnect.Common.Extensions;
using DirectCareConnect.Common.Handlers;
using DirectCareConnect.Common.Impl.Communication.DDezModels;
using DirectCareConnect.Common.Interfaces;
using DirectCareConnect.Common.Interfaces.Authentication;
using DirectCareConnect.Common.Interfaces.Communication;
using DirectCareConnect.Common.Interfaces.Storage;
using DirectCareConnect.Common.Logging;
using DirectCareConnect.Common.Models.Db;
using DirectCareConnect.Common.Models.Global;
using DirectCareConnect.Common.Models.Rest;
using DirectCareConnect.Common.Models.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Unosquare.Swan;

namespace DirectCareConnect.Common.Impl
{
    public class LoginService : ILoginService
    {
        IRestClient client;
        IMessagingService messagingService;
        IDatabaseService databaseService;
        INotifierService notifierService;

        string currentToken;
        public LoginService(IRestClient client, IMessagingService messagingService, INotifierService notifierService, IDatabaseService databaseService)
        {
            this.client = client;
            this.messagingService = messagingService;
            this.databaseService = databaseService;
            this.notifierService = notifierService;
        }


        async public Task<LoginResult> CheckLogin()
        {
            string email = String.Empty;
            try
            {
                
                var status = await this.databaseService.GetCurrentCredentialsAsync();
                if (status != null)
                {
                    email = status.Email;
                }
                else

                if (status != null)
                {

                }
                if (status != null && !status.MessagingToken.IsNullOrEmpty() && !status.Email.IsNullOrEmpty())
                {
                    await this.messagingService.Connect(status);
                    this.currentToken = status.Token;
                    try
                    {
                        await GetInitialData(status.Token);
                    }

                    catch (Exception ee)
                    {
                        await this.databaseService.Message(ee.Message);
                        await this.databaseService.Message(ee.StackTrace);
                    }

                    if (DateTime.UtcNow < status.TokenIssued.AddDays(1))
                    {
                        return new LoginResult(true) { EmailAddress = email };
                    }
                }

                return new LoginResult(false) { EmailAddress = email };
            }

            catch (Exception ee)
            {
                DebugLogger.AddLog("error" + ee.Message + " : " + ee.StackTrace);
                return new LoginResult(false) { Exception = ee.Message, EmailAddress = email };
            }
        }

        public bool ForgotPassword(string json)
        {
            _Login login = JsonConvert.DeserializeObject<_Login>(json);
            if (login.email == "victor@ibeanzinc.com")
            {
                return true;
            }

            return false;
        }

        async public Task<LoginResult> Login(string email, string password)
        {
            try
            {
                var status = await this.client.TryLogin(email, password);
                if (status.Success)
                {
                    this.currentToken = status.Token;
                    var hashedPassword = (password + email).OneWayHash();
                    var existing = await this.databaseService.GetCurrentCredentialsAsync();
                    var sendBirdToken = String.Empty;

                    if (existing == null || existing.MessagingToken.IsNullOrEmpty())
                    {
                        sendBirdToken = await this.messagingService.GetToken(email, status.Token);// await this.GetSendBirdToken(email, status.Token);
                    }
                    else
                    {
                        sendBirdToken = existing.MessagingToken;
                    }
                    DebugLogger.AddLog("got token");
                    var item = new CurrentCredentials
                    {
                        Credentials = hashedPassword,
                        LastRefreshed = DateTime.UtcNow,
                        Token = status.Token,
                        TokenIssued = DateTime.UtcNow,
                        MessagingToken = sendBirdToken,
                        Email = email
                    };
                    DebugLogger.AddLog("added token");
                    await this.databaseService.SaveCurrentCredentialsAsync(item);
                    DebugLogger.AddLog("trying to connect");
                    item.Email = "Calling from this place";
                    await this.messagingService.Connect(item);
                    //save password locally for offline as structure email,password,timestamp refreshed
                    await GetInitialData(status.Token);
                    
                }
                else
                {
                    if (!status.Exception.IsNullOrEmpty())
                    {
                        //check for network error?

                        var existingStatus = await this.databaseService.GetCurrentCredentialsAsync();
                        var hashedPassword = (password + email).OneWayHash();
                        if (existingStatus!=null && existingStatus.Email==email && existingStatus.Credentials == hashedPassword)
                        {
                            return new LoginResult(true) { EmailAddress = email };
                        }
                    }
                }
                return status;
            }

            catch (Exception ee)
            {
                return new LoginResult(false) { Exception = ee.Message };
            }
        }

        async private Task<bool> GetInitialData(string token) 
        {
            InitialData initialData = await this.client.GetInitialData(token);
            await this.databaseService.SaveInitialData(initialData); 
            var existingEntries = await this.databaseService.GetAll<ServiceEntry>();
            this.notifierService.NotifyUpdate("update dashboard");
            return true;
        }

        async public Task<Result> Login(string json)
        {
            _Login login = JsonConvert.DeserializeObject<_Login>(json);
            return await this.Login(login.email, login.password);
        }

        public Task<string> GetCurrentToken()
        {
            return Task.FromResult(this.currentToken);
        }

        async public Task LogOut()
        {
            await this.databaseService.ClearCredentials();
        }

        #region PASSWORD_RESET
        async public Task<ForgotPasswordContactModel> RequestPasswordReset(string email)
        {
            return await this.client.RequestPasswordReset(email);
        }

        async public Task<Result> SendPasswordReset(int userId, string contactMethod)
        {
             return await this.client.SendPasswordReset(userId, contactMethod);
        }

        async public Task<Result> ConfirmPasswordResetCode(int userId, string code)
        {
            return await this.client.ConfirmPasswordResetCode(userId, code);
        }

        async public Task<Result> ConfirmPasswordReset(int userId, string code, string password)
        {
           return await this.client.ConfirmPasswordReset(userId, code,password);
        }
        #endregion

        private class _Login
        {
            public string email { get; set; }
            public string password { get; set; }
        }
    }
}
