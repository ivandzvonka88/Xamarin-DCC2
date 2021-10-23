using DirectCareConnect.Blazor.Pages.Bases;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DirectCareConnect.Blazor.Components.LoadingOverlay
{
    public class LoadingOverlayBase:ModelBase
    {
        protected bool ShowOverlay { get; set; }
        [Inject] ILoadingOverlayService LoadingOverlayService { get; set; }
        protected override Task OnInitializedAsync()
        {
            this.LoadingOverlayService.ShowChanged += LoadingOverlayService_ShowChanged;
            return base.OnInitializedAsync();
        }

        private void LoadingOverlayService_ShowChanged(bool show, string message)
        {
            this.ShowOverlay = show;
            this.StateHasChanged();
        }
    }
}
