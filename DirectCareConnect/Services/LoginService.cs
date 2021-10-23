using DirectCareConnect.Common.Extensions;
using DirectCareConnect.Common.Handlers;
using DirectCareConnect.Common.Interfaces.Authentication;
using DirectCareConnect.Common.Interfaces.Communication;
using DirectCareConnect.Common.Interfaces.Storage;
using DirectCareConnect.Common.Logging;
using DirectCareConnect.Common.Models.Db;
using DirectCareConnect.Common.Models.Rest;
using DirectCareConnect.Handlers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace DirectCareConnect.Services
{
    /// <summary>
    /// TO DO MOVE THIS TO COMMON
    /// </summary>
    public class LoginService : ILoginService
    {
        IRestClient client;
        IMessagingService messagingService;
        IDatabaseService databaseService;
        
        string currentToken;
        public LoginService(IRestClient client, IMessagingService messagingService, IDatabaseService databaseService)
        {
            this.client = client;                             
             this.messagingService = messagingService;
            this.databaseService = databaseService;
        }

        public string CurrentToken
        {
            get
            {
                return this.currentToken;
            }
        }

        async public Task<bool> CheckLogin()
        {
            try
            {
               
                var status = await this.databaseService.GetCurrentCredentialsAsync();

                if (status != null && DateTime.UtcNow < status.TokenIssued.AddDays(1) && !status.MessagingToken.IsNullOrEmpty() && !status.Email.IsNullOrEmpty())
                {
                    await this.messagingService.Connect(status);
                    this.currentToken = status.Token;
                    try
                    {
                        await GetInitialData(status.Token);
                    }

                    catch(Exception ee)
                    {
                        await this.databaseService.Message(ee.Message);
                        await this.databaseService.Message(ee.StackTrace);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }

            catch (Exception ee)
            {
                DebugLogger.AddLog("error" + ee.Message + " : " + ee.StackTrace);
                return false;
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

        async public Task<bool> Login(string email, string password)
        {
            try
            {
                var status = await this.client.TryLogin(email, password);
                if (status.LoggedIn)
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
                    return true;
                }

                return false;
            }

            catch (Exception ee)
            {
                DebugLogger.AddLog("erro2r" + ee.Message + " : " + ee.StackTrace);
                return false;
            }
        }

        async private Task<bool> GetInitialData(string token)
        {
            InitialData initialData=await this.client.GetInitialData(token);
            await this.databaseService.SaveInitialData(initialData);
            LocationChangeHandler.GetLocationChangeHandler().UpdateLocations(initialData.Locations);
            return true;
        }

        async public Task<bool> Login(string json)
        {
            _Login login = JsonConvert.DeserializeObject<_Login>(json);
            return await this.Login(login.email, login.password);
        }

        private class _Login
        {
            public string email { get; set; }
            public string password { get; set; }
        }
    }
}
