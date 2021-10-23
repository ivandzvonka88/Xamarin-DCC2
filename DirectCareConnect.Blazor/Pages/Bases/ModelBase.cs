using DirectCareConnect.Blazor.Components.LoadingOverlay;
using DirectCareConnect.Common.Interfaces;
using DirectCareConnect.Common.XPlat;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DirectCareConnect.Blazor.Pages.Bases
{
    public class ModelBase: ComponentBase
    {
        [Inject] protected IXamarinBridge XamarinBridge { get; set; }
        [Inject] private ILoggingService LoggingService { get; set; }
        [Inject] private ILoadingOverlayService LoadingOverlayService { get; set; }
        [Inject] protected NavigationManager NavigationManager { get; set; }
        private Dictionary<string, bool> loading;
        protected bool ContextBusy { get; set; }

        public ModelBase()
        {
            this.loading = new Dictionary<string, bool>();
        }
        public void SetLoading(string identifier, bool loading)
        {
            if (!this.loading.ContainsKey(identifier))
            {
                this.loading.Add(identifier, loading);
            }
            else
            {
                this.loading[identifier] = loading;
            }
            this.StateHasChanged();
        }

        public void SetPageLoading(bool loading, string message=null)
        {
            if (loading)
            {
                this.LoadingOverlayService.Show(message);
            }
            else
            {
                this.LoadingOverlayService.Hide();
            }
            this.StateHasChanged();
        }

        public bool IsLoading(string property)
        {
            if (!this.loading.ContainsKey(property))
            {
                return false;
            }
            else
            {
                return this.loading[property];
            }
        }

        public void Log(string message, Dictionary<string,string> extraInfo=null)
        {
            try
            {
                Console.WriteLine($"ModelBase: {message}");
                this.LoggingService.LogInfo("ModelBase",message, extraInfo);
            }

            catch
            {
                
            }
        }
        public virtual void Dispose()
        {

        }
    }
}
