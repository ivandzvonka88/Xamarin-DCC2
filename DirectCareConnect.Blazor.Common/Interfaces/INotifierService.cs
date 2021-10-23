using BlazorMobile.Common.Attributes;
using DirectCareConnect.Common.Handlers;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DirectCareConnect.Common.Interfaces
{
   
    public interface INotifierService
    {
        event EventHandler<ModalEventHandlerEventArgs> ModalNotification;
        event EventHandler<UpdateEventHandler> UpdateNotification;
        Task<bool> Notify(string message, string title, string cancel);
        void NotifyUpdate(string message);

        void OnModalNotification(ModalEventHandlerEventArgs args);
        void OnUpdateNotification(UpdateEventHandler args);

        Task<Guid> GetId();
    }
}
