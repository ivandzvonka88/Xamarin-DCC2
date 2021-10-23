using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DirectCareConnect.Common.Interfaces.Authentication;
using DirectCareConnect.Common.Testing;
using DirectCareConnect.Common.Interfaces;
using DirectCareConnect.Common.Handlers;
using Microsoft.JSInterop;
using DirectCareConnect.Common.Interfaces.Storage;
using DirectCareConnect.Common.Models.UI;
using DirectCareConnect.Common.Models.Db.Clients;
using Microsoft.AspNetCore.Components.Rendering;
using DirectCareConnect.Common.ComponentModel;
using System.Timers;
using Newtonsoft.Json;
using DirectCareConnect.Common.Models.ComponentCommunication;
using DirectCareConnect.Common.Interfaces.Communication;
using DirectCareConnect.Common.Models.Db;
using Blazored.Toast.Services;
using DirectCareConnect.Blazor.Pages.Bases;
using DirectCareConnect.Blazor.Components.LoadingOverlay;
using BlazorMobile.Common.Services;
using DirectCareConnect.Common.XPlat;
using DirectCareConnect.Common.Constants;

namespace DirectCareConnect.Blazor.Pages
{
    public class DashboardBase : ModelBase, IDisposable
    {
        private string RestToken;
        
        [Inject] ILoginService LoginService { get; set; }
        [Inject] IDatabaseService DbService { get; set; }
        
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] IRestClient RestClient { get; set; }

        [Inject] IModalService ModalService { get; set; }
        [Inject] IJobService JobService { get; set; }
      
        [Inject] INotifierService Notifier { get; set; }

        [Inject] IToastService toastService { get; set; }
        
        private DashboardModel model { get; set; }

        private Timer sessionElapsed { get; set; }

        private Guid instanceId { get; set; }

        public DashboardBase() : base()
        {
            this.Model = new DashboardModel();
            this.sessionElapsed = new Timer();
            this.sessionElapsed.Interval = 1000;
            this.sessionElapsed.Elapsed += SessionElapsed_Elapsed;
            this.Model.ShowHeader = true;
            this.Model.ShowDashboard = true;
            this.instanceId = Guid.NewGuid();
        }

        private void NavigationManager_LocationChanged(object sender, Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs e)
        {
           
        }

        private void SessionElapsed_Elapsed(object sender, ElapsedEventArgs e)
        {
            InvokeAsync(() =>
            {
                this.Model.SessionTimeInSeconds++;
                base.StateHasChanged();
            });
        }

        async protected override Task OnInitializedAsync()
        {
            this.Log("DCC-INITA");
            this.SetPageLoading(true,"Test");
            //this.toastService.ShowInfo("Initializing: " + this.instanceId);
            NavigationManager.LocationChanged += NavigationManager_LocationChanged;
            BlazorMobileService.MessageSubscribe<string>("Update", OnMessageReceived);
            this.Log("DCC-INIT");
            var creds = await this.DbService.GetCurrentCredentialsAsync();
            if (creds == null)
            {
                
                NavigationManager.NavigateTo("/login");
                return;
            }
            this.RestToken = (creds).Token;
            if (this.RestToken == "" || this.RestToken == null)
            {
                NavigationManager.NavigateTo("/login");
                return;
            }

            await base.OnInitializedAsync();

            
            

            BlazorMobile.Common.Services.BlazorMobileService.MessageSubscribe<string>("myNotification", OnMessageReceived);

            

            try
            {
                this.Model = await DbService.GetDashboardModel();
                if(this.Model.SessionTimeInSeconds.HasValue)
                {
                    this.sessionElapsed.Stop();
                    this.sessionElapsed.Start();
                }
            }

            catch
            {
            

            }

            
            
            ModalService.OnClose += ModalService_OnClose;
            this.SetPageLoading(false, "Test");
        }


        async protected Task Logout()
        {
            await this.LoginService.LogOut();
            NavigationManager.NavigateTo("/");
        }



        async private void ModalService_OnClose(object model)
        {
            //this seems to be necessary to allow things to close, etc
            try
            {
                this.SetPageLoading(true);
                this.ContextBusy = true;
                this.Log($"Closing:getting the model");
                this.Model = await DbService.GetDashboardModel();
                this.Log($"Closing:got the model");
                StateHasChanged();
                this.Log($"Closing:changing the state");
                
                this.sessionElapsed.Stop();
                //if (this.Model.SessionTimeInMinutes)
                this.sessionElapsed.Start();

                this.StateHasChanged();

                
                this.Log($"Closing: {model.GetType().FullName}");

                if (model is EndServiceModel)
                {
                    this.Log($"Closing:updating");
                    var updated = await this.RestClient.UpdateWebServer();
                    this.Log($"Closing:updating done");
                    if (!updated)
                    {
                        this.Log($"Closing:updating error");
                        this.toastService.ShowError("Session saved locally, unable to send to server.");
                    }

                    this.Log($"Closing:updating no error");
                    EndServiceModel endServiceModel = model as EndServiceModel;

                    this.Log($"Closing:updating getting item");
                    if (this.DbService == null)
                    {
                        this.Log($"Closing:getting item db service is null");
                    }
                    else
                    {
                        this.Log($"Closing:getting item db service is not null {endServiceModel.ClientId} {endServiceModel.ServiceId}");
                    }

                    var _item = await this.DbService.GetAll<ClientService>();

                    if (_item == null)
                    {
                        this.Log($"Closing:getting _ item is null");
                        this.toastService.ShowError($"An error has occurred. item is null");
                        return;
                    }
                    this.Log($"Closing:updating got item1");
                    var _item2 = _item.Where(p => p.ClientId == endServiceModel.ClientId && p.ServiceId == endServiceModel.ServiceId);

                    this.Log($"Closing:updating got item");
                    var item = _item2.FirstOrDefault();
                    this.Log($"Closing:updating got item");
                    try
                    {
                        string url = String.Empty;
                        url = $"endsession{item.ServiceName.ToLower()}/{endServiceModel.ClientId}/{endServiceModel.ClientServiceId}/0/0";
                        this.Log($"Closing:going to {url}");
                        NavigationManager.NavigateTo(url);
                    }

                    catch (Exception ee)
                    {
                        this.toastService.ShowError($"An error has occurred. {ee.Message}");
                    }


                }
                else if (model is StartServiceModel)
                {
                    this.toastService.ShowSuccess("Session Started");
                }
                else if(model is string)
                {
                    if(model.ToString()== Messaging.Cancel)
                    {
                        var loadingKey = nameof(EndSession);
                        this.SetLoading(loadingKey, false);
                    }
                }
            }


            catch { }

            finally
            {
                this.SetPageLoading(false);
                this.ContextBusy = false;
            }
        }


        async public void OnMessageReceived(string payload)
        {
            if (this.ContextBusy)
                return;
           
            try
            {
                this.Log(payload, null);
                if (payload == "update dashboard")
                {
                    this.Log("Getting db model");
                    if (this.DbService == null)
                    {
                        this.Log($"Closing:getting item db service is null");
                    }
                    else
                    {
                        this.Model = await DbService.GetDashboardModel();

                    }

                    this.Log("Got db model");
                    //this.toastService.ShowSuccess("Dashboard has been updated" + this.instanceId);
                    StateHasChanged();
                    this.Log("changed state");
                }
                else
                {
                    NavigationManager.NavigateTo("login");
                }

            }
            catch(Exception ee)
            {
                this.Log(ee.Message);
            }
        }

        async public void StartSession()
        {
            var loadingKey = nameof(StartSession);
           
            this.SetLoading(loadingKey, true);
            try
            {
                var typ = typeof(DirectCareConnect.Blazor.Components.StartService);
                StartServiceModel model = await JobService.GetStartServiceModel();
                
                ModalService.Show(String.Empty, typ, model);
            }

            catch
            {
                
            }

            finally
            {
                this.SetLoading(loadingKey, false);
            }
        }

        async public void EndSession()
        {
            var loadingKey = nameof(EndSession);
            this.SetLoading(loadingKey, true);
            var updated=await this.RestClient.UpdateWebServer();
            if (!updated)
            {
                this.toastService.ShowError("Cannot connect to server, updating session locally");
            }

            var typ = typeof(DirectCareConnect.Blazor.Components.EndService);
            StartServiceModel model = await JobService.GetEndServiceModel();
            ModalService.Show(String.Empty, typ, model);
           // this.SetLoading(loadingKey, false);
            return;
        }

        public void ShowClientAlerts()
        {
            HideViews();
            this.Model.ShowClientAlerts = true;

        }
        public void ShowDocumentation()
        {
            HideViews();
            this.Model.ShowDocumentation = true;

        }
        public void ShowCredentials()
        {
            HideViews();
            this.Model.ShowCredentials = true;

        }
        async public void Test()
        {
            var loadingKey = nameof(Test);
            
            this.SetLoading(loadingKey, true);
            await this.LoginService.CheckLogin();
            await this.RestClient.UpdateWebServer();
            try
            {
                this.Model = await DbService.GetDashboardModel();
                if (this.Model.SessionTimeInSeconds.HasValue)
                {
                    this.sessionElapsed.Stop();
                    this.sessionElapsed.Start();
                }
            }

            catch
            {


            }
            this.SetLoading(loadingKey, false);
           // this.toastService.ShowSuccess("Server Updated");
            StateHasChanged();

        }

        public override void Dispose()
        {
            ModalService.OnClose -= ModalService_OnClose;
            NavigationManager.LocationChanged -= NavigationManager_LocationChanged;
            BlazorMobileService.MessageUnsubscribe<string>("Update", OnMessageReceived);
            BlazorMobileService.MessageUnsubscribe<string>("myNotification", OnMessageReceived);
           // this.Log("Disposing: " + this.instanceId);
            base.Dispose();
        }

        public string SessionTime { 
            get
            {
                if (Model == null || !Model.SessionTimeInSeconds.HasValue)
                    return String.Empty;

                var ts = TimeSpan.FromSeconds(Model.SessionTimeInSeconds.Value);
                return ts.ToString("hh\\:mm\\:ss");

            } 
        }

        public string WeeklyAverage
        {
            get
            {
                if (Model == null || !Model.WeeklyAvgAllowedInMinutes.HasValue)
                    return String.Empty;

                return Model.WeeklyAvgAllowedInMinutes.ToString();
                

            }
        }

        public DashboardModel Model
        {
            get
            {
                return this.model;
            }

            set
            {
                this.model = value;
                ResetView();
            }
        }

        public void ResetView()
        {
            if (this.Model == null)
                return;

            HideViews();
            this.Model.ShowDashboard = true;
            this.Model.ShowHeader = true;
        }

        public void HideViews()
        {
            this.Model.ShowDashboard = false;
            this.Model.ShowHeader = false;
            this.Model.ShowClientAlerts = false;
            this.Model.ShowCredentials = false;
            this.Model.ShowDocumentation = false;
        }
    }
}
