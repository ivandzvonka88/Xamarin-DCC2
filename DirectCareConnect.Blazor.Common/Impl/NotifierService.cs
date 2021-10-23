using DirectCareConnect.Common.Handlers;
using DirectCareConnect.Common.Impl;
using DirectCareConnect.Common.Interfaces;
using DirectCareConnect.Common.Logging;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;



namespace DirectCareConnect.Common.Impl
{
    /// <summary>
    /// Confirm this is not used, and remove
    /// </summary>
    /// 
    
    public class NotifierService: INotifierService, IDisposable
    {
        IXamarinBridge bridge;
        IJSRuntime jsRuntime;
        IModalService modal;
        Guid id;
        
        public NotifierService()
        {
            id = Guid.NewGuid();

        }

        public void SetJSRuntime(IJSRuntime runtime)
        {
            this.jsRuntime = runtime;
        }

        public void SetXamarinBridge(IXamarinBridge bridge)
        {
            this.bridge = bridge;
        }
        public void SetModalService(IModalService modal)
        {
            this.modal = modal;
        }


        public event EventHandler<ModalEventHandlerEventArgs> ModalNotification;
        public event EventHandler<UpdateEventHandler> UpdateNotification;

        private void Bridge_OnAlert(string title, string stringType, bool isModal)
        {
            if (isModal)
            {
                //ModalService.Show("Simple Form", typeof(EnterFence));
            }
        }

        async public Task<bool> Notify(string message, string title, string cancel)
        {
            if (Global.IsXamarin)
            {
                await bridge.DisplayAlert(message, title, cancel);
            }
            else
            {
                await jsRuntime.InvokeVoidAsync("ShowAlert", message);
            }

            return true;
        }

        public void Dispose()
        {
           // this.bridge.OnAlert -= Bridge_OnAlert;
        }


        async public Task<Guid> GetId()
        {
            return await Task.FromResult(this.id);
        }

        public void OnModalNotification(ModalEventHandlerEventArgs args)
        {
            ModalNotification?.Invoke(this, args);
        }

        public void NotifyUpdate(string message)
        {
            OnUpdateNotification(new UpdateEventHandler { UpdateMessage = message });

        }

        public void OnUpdateNotification(UpdateEventHandler args)
        {
            UpdateNotification?.Invoke(this, args);
        }
    }
}
