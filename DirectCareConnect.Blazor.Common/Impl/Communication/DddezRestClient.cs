using DirectCareConnect.Common.Config;
using DirectCareConnect.Common.Impl.Communication.DDezModels;
using DirectCareConnect.Common.Interfaces.Communication;
using DirectCareConnect.Common.Interfaces.Storage;
using DirectCareConnect.Common.Models.Rest;
using DirectCareConnect.Common.Models.UI;
using DirectCareConnect.Common.Models.Users;
using DirectCareConnect.Common.Models.Db;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Er = DirectCareConnect.Common.Models.Rest.Er;
using Xamarin.Forms.Internals;
using DirectCareConnect.Common.Constants;
using DirectCareConnect.Common.Handlers;
using System.Threading;
using DirectCareConnect.Common.Extensions;
using DirectCareConnect.Common.Interfaces;
using DirectCareConnect.Common.XPlat;
using DirectCareConnect.Common.Models.Global;

namespace DirectCareConnect.Common.Impl.Communication
{
    public class DddezRestClient : IRestClient
    {
        IDatabaseService databaseService;
        INotifierService notifierService;
        ILoggingService bridge;
        private readonly HttpClient client;// = new HttpClient();
        string serviceUrl = "";
        public DddezRestClient(IDatabaseService databaseService, INotifierService notifierService, ILoggingService bridge)
        {
            this.databaseService = databaseService;
            this.notifierService = notifierService;
            serviceUrl = AppSettingsManager.Settings["Service"];
            this.bridge = bridge;

            #if Debug

                        /*     // First create a proxy object
                     var proxy = new WebProxy
                     {
                         Address = new Uri("http://192.168.1.105:8888"),
                         BypassProxyOnLocal = false,
                         UseDefaultCredentials = true
                     };
                     */
                        var httpClientHandler = new TimeoutHandler
            
                        {
                            InnerHandler = new HttpClientHandler
                            {
                                //Proxy = proxy,
                                ClientCertificateOptions = ClientCertificateOption.Manual,
                                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
                                {
                                    //bypass
                                    return true;
                                }

                            }
                        };
          

                        // Finally, create the HTTP client object
                        this.client = new HttpClient(handler: httpClientHandler, disposeHandler: false) { Timeout = TimeSpan.FromSeconds(7) };
                        #else
                this.client= new HttpClient();
            #endif
        }
        async public Task<InitialData> GetInitialData(string token)
        {
            try
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Content", "application/x-www-form-urlencoded");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);

                var request = new HttpRequestMessage(HttpMethod.Get, $"{this.serviceUrl}api/providerconfiguration");
                request.SetTimeout(TimeSpan.FromSeconds(60));
                HttpResponseMessage ans = await client.SendAsync(request);
                var jsonAns = await ans.Content.ReadAsStringAsync();
                ProviderConfiguration answer = ProviderConfiguration.FromJson(jsonAns);
                return answer.ToInitialData();
            }


            catch(Exception ee)
            {
                return null;
            }
        }

        async public Task<ClientNote> GetNote(string token, string companyId, string documentId, string docType)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Content", "application/x-www-form-urlencoded");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);
            var ans = await client.GetAsync($"{this.serviceUrl}api/notes/getnote?coId={companyId}&docId={documentId}&docType={docType}");
            var jsonAns = await ans.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ClientNote>(jsonAns);
        }

        public Task<List<User>> GetUsers(string token)
        {
            throw new NotImplementedException();
        }

        async public Task<SetNoteResponse> SetNote(string token,  EndSessionRespiteModel model)
        {
            ClientNote note = CreateClientNote(model);
            return await this.SetNote(token, model, note);
        }

        async public Task<SetNoteResponse> SetNote(string token, EndSessionHabilitationModel model)
        {
            ClientNote note = CreateClientNote(model);
            return await this.SetNote(token, model, note);
        }

        async public Task<SetNoteResponse> SetNote(string token, EndSessionAttendantCareModel model)
        {
            ClientNote note = CreateClientNote(model);
            return await this.SetNote(token, model, note);
        }
        async public Task<LoginResult> TryLogin(string email, string password)
        {
            try
            {
                client.DefaultRequestHeaders.Accept.Clear();
                string json = $"Username={email}&Password={password}&grant_type=password&deviceId=AAAA";
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var ans = await client.PostAsync($"{this.serviceUrl}token", content);
                var jsonAns = await ans.Content.ReadAsStringAsync();
                loginAnswer answer = JsonConvert.DeserializeObject<loginAnswer>(jsonAns);
                if (answer.error == null && answer.access_token != null)
                {
                    return new LoginResult
                    {
                        Success = true,
                        Token = answer.access_token
                    };
                }

                return new LoginResult(false);
            }

            catch(Exception ee)
            {
                return new LoginResult { Success = false, Exception=ee.Message };

            }

            
        }

        async private Task<SetNoteResponse> SetNote(string token, EndSessionModelBase model,ClientNote note)
        {
            try
            {

                //TODO add timeouts
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);



                var requestContent = new MultipartFormDataContent();

                if (model.FileId != Guid.Empty)
                {
                    var imageContent = new ByteArrayContent(model.FileBuffer);
                    imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");

                    requestContent.Add(imageContent, "attachment", model.FileName);
                }

                var contentToSend = JsonConvert.SerializeObject(note);
                var stringContent = new StringContent(contentToSend);
                stringContent.Headers.Add("Content-Disposition", "form-data; name=\"_sessionNote\"");
                requestContent.Add(stringContent, "json");


                var ans = await client.PostAsync($"{this.serviceUrl}api/notes/setnote", requestContent);
                var jsonAns = await ans.Content.ReadAsStringAsync();
                var pendingDocumentations = JsonConvert.DeserializeObject<List<PendingDocumentation>>(jsonAns);
                SetNoteResponse answer = new SetNoteResponse();
                answer.PendingDocumentations = pendingDocumentations;
                //todo update pending
                if (ans.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    answer.Er = new DDezModels.Er { Code = 0 };
                }
                else
                {
                    answer.Er = new DDezModels.Er { Code = -1, Msg = ans.StatusCode };
                }

                return answer;
            }


            catch
            {
                return null;
            }
        }




        private ClientNote CreateClientNote(EndSessionRespiteModel model)
        {
            ClientNote note = CreateClientNote(model as EndSessionModelBase);
            note.docType = DirectCareConnect.Common.Constants.Notes.RSPServiceNote;
            return note;
        }
        private ClientNote CreateClientNote(EndSessionHabilitationModel model)
        {
            ClientNote note = CreateClientNote(model as EndSessionModelBase);
            note.longTermObjectives = model.LongTermObjectives;
            note.docType = DirectCareConnect.Common.Constants.Notes.HAHServiceNote;
            return note;
        }

        private ClientNote CreateClientNote(EndSessionOccupationalTherapyModel model)
        {
            ClientNote note = CreateClientNote(model as EndSessionModelBase);
            note.longTermObjectives = model.LongTermObjectives;
            note.docType = DirectCareConnect.Common.Constants.Notes.TherapyServiceNote;
            return note;
        }

        private ClientNote CreateClientNote(EndSessionAttendantCareModel model)
        {
            ClientNote note = CreateClientNote(model as EndSessionModelBase);
            note.careAreas = model.CareAreas;
            note.docType= DirectCareConnect.Common.Constants.Notes.ATCServiceNote;
            return note;
        }

        private ClientNote CreateClientNote(EndSessionModelBase model)
        {
            ClientNote note = new ClientNote
            {
                attachmentName = model.AttachmentName,
                docId = model.DocId,
                noShow = model.NoShow,
                note = model.Note,
                providerId = model.ProviderId,
                coId = model.CompanyId.ToString(),
                completed=model.Completed
            };

            switch(model.Resolution)
            {
                case NoteResolution.ClientRefusedWork:
                    note.clientRefusedService = true;
                    break;
                case NoteResolution.DesigneeRefusedToSign:
                    note.designeeRefusedToSign = true;
                    break;
                case NoteResolution.DesigneeSign:
                    note.designeeId = System.Convert.ToInt32(model.SelectedDesignee.Split('-')[0]);
                    note.guardianId = System.Convert.ToInt32(model.SelectedDesignee.Split('-')[1]);
                    break;
                case NoteResolution.DesigneeUnableToSign:
                    break;
                case NoteResolution.NoShow:
                    break;
                case NoteResolution.UnsafeToWork:
                    break;

            }

            return note;
        }

        async public Task<bool> SendServiceUpdate(string token, ServiceEntry model)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);




            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");


                var ans = await client.PostAsync($"{this.serviceUrl}api/InOut/SessionInfo", content);
                var jsonAns = await ans.Content.ReadAsStringAsync();
                Er answer = JsonConvert.DeserializeObject<Er>(jsonAns);
                if (ans.StatusCode == System.Net.HttpStatusCode.OK && answer.code == 0)
                    return true;



            return false;
        }
        async public Task<bool> UpdateWebServer()
        {
            try
            {
                await this.bridge.LogError("DCC","DCC-Called Update");
                var db = (await databaseService.GetCurrentCredentialsAsync());
                if(db==null)
                {
                    await this.bridge.LogError("DCC", "DCC-Called Update NULL");
                    return false;
                }
                string token = db.Token;
                await  this.bridge.LogError("DCC", "DCC-Called Update-Token");
                bool successfull = true;
                bool needRefresh = false;
                //XXXXXXXXXX - we want to keep session info in order
                // First Send All session Started but not ended
                var openServices = (await this.databaseService.GetAll<ServiceEntry>()).Where(p => !p.StartAcknowledged).OrderBy(p=>p.ServiceEntryId);
                await this.bridge.LogError("DCC", "DCC-Called Update-Services");
                foreach (var service in openServices)
                {
                    needRefresh = true;
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);
                    var contentToSend = service.ToJsonString();
                    var content = new StringContent(contentToSend, Encoding.UTF8, "application/json");

                    var request = new HttpRequestMessage(HttpMethod.Post, $"{this.serviceUrl}api/InOut/SessionInfo");
                    request.SetTimeout(TimeSpan.FromSeconds(60));
                    request.Content = content;

                    var ans = await client.SendAsync(request);
                    var jsonAns = await ans.Content.ReadAsStringAsync();
                    SendOpenSessionResponse answer = JsonConvert.DeserializeObject<SendOpenSessionResponse>(jsonAns);
                    if (ans.StatusCode != System.Net.HttpStatusCode.OK || answer == null || answer.er.Code != 0)
                    {
                        // log error
                        successfull = false;
                        break;
                    }
                    else
                    {
                        // update the session info
                        service.StartAcknowledged = true;
                        if (answer.HCBSEmpHrsId != 0)
                        {
                            service.RemoteServiceEntryId = answer.HCBSEmpHrsId;
                            await this.databaseService.UpdateSavedServiceEntryNotes(service.ServiceEntryId, answer.HCBSEmpHrsId);
                        }

                        await this.databaseService.Update(service);

                    }
                }

                if (successfull)
                {
                    //XXXXXXXXX send all ended sessions
                    var closedServices = (await this.databaseService.GetAll<ServiceEntry>()).Where(p => p.EndUTC != null && p.StartAcknowledged &&  !p.EndAcknowledged);
                    foreach (var service in closedServices)
                    {
                        needRefresh = true;
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);
                        var contentToSend = service.ToJsonString();
                        var content = new StringContent(contentToSend, Encoding.UTF8, "application/json");


                        var request = new HttpRequestMessage(HttpMethod.Post, $"{this.serviceUrl}api/InOut/SessionInfo");
                        request.SetTimeout(TimeSpan.FromSeconds(60));
                        request.Content = content;

                        var ans = await client.SendAsync(request);

                        var jsonAns = await ans.Content.ReadAsStringAsync();
                        Er answer = JsonConvert.DeserializeObject<Er>(jsonAns);
                        if (ans.StatusCode != System.Net.HttpStatusCode.OK || answer.code != 0)
                        {
                            // log error
                            successfull = false;
                            break;
                        }
                        else
                        {
                            // update the session info - maybe we can delete it
                            service.EndAcknowledged = true;
                            await this.databaseService.Update(service);

                        }
                    }
                }

                if (needRefresh)
                {
                    var data = await this.GetInitialData(token);
                    await this.databaseService.SaveInitialData(data);
                    this.notifierService.NotifyUpdate("update dashboard");

                }
                return successfull;
            }
            catch(Exception ee)
            {
                await this.bridge.LogError("DCC", "DCC-Called Update-" + ee.Message);
                return false;
            }
            finally
            {
                await this.bridge.LogError("DCC", "DCC-Called Update-Fihally");
            }
        }

        async public Task<SetNoteResponse> SetNote(string token, EndSessionOccupationalTherapyModel model)
        {
            ClientNote note = CreateClientNote(model);
            return await this.SetNote(token, model, note);
        }
        async public Task<SetNoteResponse> SetNote(string token, EndSessionPhysicalTherapyModel model)
        {
            ClientNote note = CreateClientNote(model);
            return await this.SetNote(token, model, note);
        }

        async public Task<List<SchedulerEvent>> GetSchedule(string token, string providerId, string companyId, DateTime start, DateTime to)
        {
            string jsonAns = String.Empty;
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Content", "application/x-www-form-urlencoded");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);
            try
            {
                var ans = await client.GetAsync($"{this.serviceUrl}api/calendar/GetData?companyId={companyId}&providerId={providerId}&from={start}&to={to}");
                jsonAns = await ans.Content.ReadAsStringAsync();
            }

            catch(Exception ee)
            {

            }
            return JsonConvert.DeserializeObject<List<SchedulerEvent>>(jsonAns);
        }

        #region PASSWORD_RESET
        async public Task<ForgotPasswordContactModel> RequestPasswordReset(string email)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            string json = $"email={email}";
            var content = new StringContent(json, Encoding.UTF8, "application/x-www-form-urlencoded");
            var ans = await client.PostAsync($"{this.serviceUrl}api/ProviderConfiguration/ResetPassword", content);
            var jsonAns = await ans.Content.ReadAsStringAsync();
            try
            {
                var result = JsonConvert.DeserializeObject<ForgotPasswordContactModel>(jsonAns);
                result.Success = true;
                return result;
            }
            catch(Exception ee)

            {
                return new ForgotPasswordContactModel
                {
                    Success = false,
                    Error = ee.Message
                };
            }
        }

        async public Task<Result> SendPasswordReset(int userId, string contactMethod)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            string json = $"userId={userId}&contactMethod={contactMethod}";
            var content = new StringContent(json, Encoding.UTF8, "application/x-www-form-urlencoded");
            var ans = await client.PostAsync($"{this.serviceUrl}api/ProviderConfiguration/ResetPasswordContact", content);
            var jsonAns = await ans.Content.ReadAsStringAsync();
            try
            {
                var result = JsonConvert.DeserializeObject<ForgotPasswordContactModel>(jsonAns);
                
                return new Result(true);
            }
            catch (Exception ee)

            {
                return new Result
                {
                    Success = false,
                    Exception = ee.Message
                };
            }
        }

        async public Task<Result> ConfirmPasswordResetCode(int userId, string code)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            string json = $"userId={userId}&code={code}";
            var content = new StringContent(json, Encoding.UTF8, "application/x-www-form-urlencoded");
            var ans = await client.PostAsync($"{this.serviceUrl}api/ProviderConfiguration/ResetPasswordConfirmCode", content);
            var jsonAns = await ans.Content.ReadAsStringAsync();
            try
            {
                var result = JsonConvert.DeserializeObject<ForgotPasswordContactModel>(jsonAns);
                return new Result(true);
            }
            catch (Exception ee)

            {
                return new Result
                {
                    Success = false,
                    Exception = ee.Message
                };
            }
        }

        async public Task<Result> ConfirmPasswordReset(int userId, string code, string password)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            string json = $"userId={userId}&code={code}&password={password}";
            var content = new StringContent(json, Encoding.UTF8, "application/x-www-form-urlencoded");
            var ans = await client.PostAsync($"{this.serviceUrl}api/ProviderConfiguration/ResetPasswordConfirm", content);
            var jsonAns = await ans.Content.ReadAsStringAsync();
            try
            {
                var result = JsonConvert.DeserializeObject<ForgotPasswordContactModel>(jsonAns);
                return new Result(true);
            }
            catch (Exception ee)

            {
                return new Result
                {
                    Success = false,
                    Exception = ee.Message
                };
            }
        }
        #endregion
        private class loginAnswer
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public string error { get; set; }
            public string error_description { get; set; }
            public Int64 expires_in { get; set; }
        }
    }
}
