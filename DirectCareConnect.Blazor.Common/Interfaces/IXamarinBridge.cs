using BlazorMobile.Common.Attributes;
using DirectCareConnect.Common.Models.ComponentCommunication;
using DirectCareConnect.Common.Testing;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DirectCareConnect.Common.Interfaces
{
    [ProxyInterface]
    public interface IXamarinBridge
    {
        Task<List<string>> DisplayAlert(string title, string msg, string cancel);
        
        //event Action<string, string, bool> OnAlert;
        Task<FilePickerResult> LaunchFilePicker();
    }
}
