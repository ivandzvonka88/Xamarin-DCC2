using DirectCareConnect.Blazor.Pages.Bases;
using DirectCareConnect.Common.Interfaces.Communication;
using DirectCareConnect.Common.Interfaces.Storage;
using DirectCareConnect.Common.Models.Db;
using DirectCareConnect.Common.Models.UI;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DirectCareConnect.Blazor.Pages
{
    public class CalendarBase:ModelBase, IDisposable
    {
        [Inject] IRestClient RestClient { get; set; }
        [Inject] IDatabaseService DbService { get; set; }
        CalendarModel model;
        CurrentCredentials creds;
        Company company;
        async protected override Task OnInitializedAsync()
        {
            
            this.creds = await this.DbService.GetCurrentCredentialsAsync();
            
            if (creds == null)
            {
                NavigationManager.NavigateTo("/login");
                return;
            }
            this.company = (await this.DbService.GetCompany());
            this.Log("gettin events");
            this.Model.Events = await this.RestClient.GetSchedule(creds.Token, company.ProviderId.ToString(), company.CompanyId.ToString(), this.Model.StartDate, this.Model.EndDate);
            this.Log("go events");
            await base.OnInitializedAsync();
        }

        private void InitializeModel()
        {
            this.Log("Init mods");
            DayOfWeek day = DateTime.Now.DayOfWeek;
            int days = day - DayOfWeek.Monday;
            DateTime start = DateTime.Now.AddDays(-days);

            this.model = new CalendarModel
            {
                StartDate = start,
                EndDate = start.AddDays(6)
            };
        }

        public CalendarModel Model
        {
            get
            {
                this.Log("Init getting model");
                if (this.model == null)
                    InitializeModel();

                return this.model;
            }

            set
            {
                this.model = value;
            }
        }

        public void GoBack()
        {
            NavigationManager.NavigateTo("/dashboard");
        }

        async public void MoveLastWeek()
        {
            this.SetLoading(nameof(MoveLastWeek), true);
            this.model.StartDate = this.model.StartDate.AddDays(-7);
            this.model.EndDate = this.model.EndDate.AddDays(-7);
            this.Model.Events = await this.RestClient.GetSchedule(creds.Token, company.ProviderId.ToString(), company.CompanyId.ToString(), this.Model.StartDate, this.Model.EndDate);
            this.SetLoading(nameof(MoveLastWeek), false);
        }

        async public void MoveNextWeek()
        {
            this.SetLoading(nameof(MoveNextWeek), true);
            this.model.StartDate = this.model.StartDate.AddDays(7);
            this.model.EndDate = this.model.EndDate.AddDays(7);
            this.Model.Events = await this.RestClient.GetSchedule(creds.Token, company.ProviderId.ToString(), company.CompanyId.ToString(), this.Model.StartDate, this.Model.EndDate);
            this.SetLoading(nameof(MoveNextWeek), false);
        }
    }
}
