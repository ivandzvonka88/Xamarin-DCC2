using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DirectCareConnect.Blazor.Components.Buttons
{
    public class GoBackComponentBase: ComponentBase
    {
        [Parameter]
        public EventCallback<bool> OnGoBack { get; set; }

        [Parameter]
        public string HeaderText{ get; set; }

        public async void GoBack()
        {
            await OnGoBack.InvokeAsync(true);
        }

    }
}
