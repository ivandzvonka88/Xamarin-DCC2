using DirectCareConnect.Blazor.Pages.Bases;
using DirectCareConnect.Common.Interfaces;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DirectCareConnect.Blazor.Components
{
    public class ModalBase: ModelBase, IDisposable
    {
        [Inject] IModalService ModalService { get; set; }
        [Inject] NavigationManager NavigationManager { get; set; }
        protected bool IsVisible { get; set; }
        protected string Title { get; set; }
        protected RenderFragment Content { get; set; }

        protected override void OnInitialized()
        {
            ModalService.OnShow += ShowModal;
            ModalService.OnClose += CloseModal;
        }

        public void ShowModal(string title, RenderFragment content)
        {
            if (IsVisible || this.NavigationManager.Uri.ToLower().IndexOf("dashboard") == -1)
            {
                this.Log($"Not showing pop-up for{this.NavigationManager.Uri}");
                return;
            }
            Title = title;
            Content = content;
            IsVisible = true;
            StateHasChanged();
        }

        public void CloseModal(object model)
        {
            IsVisible = false;
            Title = "";
            Content = null;
            StateHasChanged();
        }

        public override void Dispose()
        {
            ModalService.OnShow -= ShowModal;
            ModalService.OnClose -= CloseModal;
        }
    }
}
