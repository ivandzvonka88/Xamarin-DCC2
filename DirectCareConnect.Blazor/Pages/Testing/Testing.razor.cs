using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DirectCareConnect.Common.Handlers;
using DirectCareConnect.Common.Interfaces;
using DirectCareConnect.Common.Models.UI;
using Microsoft.AspNetCore.Components;


namespace DirectCareConnect.Blazor.Pages.Testing
{
    public class TestingBase: ComponentBase
    {
        protected TestingModel model = new TestingModel();
        [Inject] IJobService JobService { get; set; }
        [Inject] IXamarinBridge XamarinBridge { get; set; }
        [Inject] NavigationManager NavigationManager { get; set; }

        public void EnterLocation()
        {
            NavigationManager.NavigateTo("dashboard");

        }
    }
}
