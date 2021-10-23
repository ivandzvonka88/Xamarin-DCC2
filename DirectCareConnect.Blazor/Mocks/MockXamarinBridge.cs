using DirectCareConnect.Common.Impl;
using DirectCareConnect.Common.Interfaces;
using DirectCareConnect.Common.Models.ComponentCommunication;
using DirectCareConnect.Common.Testing;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DirectCareConnect.Blazor.Mocks
{
    public class MockXamarinBridge : IXamarinBridge
    {
        
        public MockXamarinBridge()
        {
        
        }

        public Task<List<string>> DisplayAlert(string title, string msg, string cancel)
        {
            return Task.FromResult(new List<string>());
        }


        public Task<FilePickerResult> LaunchFilePicker()
        {
            throw new NotImplementedException();
        }

    }
}
