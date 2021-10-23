using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DirectCareConnect.Blazor.Components.Buttons
{
    public class SpinningButtonBase: ComponentBase
    {
        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        [Parameter]
        public string ButtonText { get; set; }

        [Parameter]
        public string ButtonType { get; set; }

        [Parameter]
        public string ButtonClass{ get; set; }

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public bool Loading { get; set; }

        public void OnClickInternal(MouseEventArgs e)
        {
            if(!this.Loading){
                OnClick.InvokeAsync(e);
            }
        }

    }
}
