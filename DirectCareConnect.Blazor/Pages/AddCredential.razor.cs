using DirectCareConnect.Common.Interfaces.Storage;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DirectCareConnect.Blazor.Pages
{
    public class AddCredentialModelBase : ComponentBase, IDisposable
    {
        [Inject] NavigationManager NavigationManager { get; set; }
        [Inject] IDatabaseService DbService { get; set; }
        [Parameter]
        public string Pid { get; set; }

        [Parameter]
        public string CredTypeId { get; set; }

        protected override Task OnInitializedAsync()
        {
            /*
            this.RestToken = (await this.DbService.GetCurrentCredentialsAsync()).Token;
            if (this.RestToken == "" || this.RestToken == null)
            {
                NavigationManager.NavigateTo("/login");
                return;
            }
            */
            return base.OnInitializedAsync();
        }
        public void Dispose()
        {
            
        }
        public void GoBack()
        {
            NavigationManager.NavigateTo("/dashboard");
        }

    }
}
