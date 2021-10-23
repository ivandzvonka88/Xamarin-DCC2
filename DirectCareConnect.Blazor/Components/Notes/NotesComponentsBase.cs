using DirectCareConnect.Blazor.Pages.Bases;
using DirectCareConnect.Common.Interfaces;
using DirectCareConnect.Common.Models.Rest;
using DirectCareConnect.Common.Models.UI;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DirectCareConnect.Blazor.Components.Notes
{
    public class NotesComponentsBase : ModelBase
    {

        [Parameter]
        public EndSessionModelBase EndSessionModel { get; set; }

        [Parameter]
        public EventCallback<bool> OnInputReceived { get; set; }

        public async void InputReceived()
        {
            await OnInputReceived.InvokeAsync(true);
        }

    }
}
