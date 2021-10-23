using DirectCareConnect.Common.Interfaces;
using DirectCareConnect.Common.Interfaces.Authentication;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DirectCareConnect.Blazor.Pages
{
    public class IndexBase: ComponentBase
    {
        [Inject] ILoginService LoginService { get; set; }
        [Inject] NavigationManager NavigationManager { get; set; }
        [Inject] IXamarinBridge XamarinBridge { get; set; }
       
    }
}
