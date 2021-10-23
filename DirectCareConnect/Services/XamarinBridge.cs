using BlazorMobile.Common;
using DirectCareConnect.Common.Interfaces;
using DirectCareConnect.Common.Testing;
using DirectCareConnect.Services;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AppCenter.Analytics;
using Plugin.FilePicker.Abstractions;
using Plugin.FilePicker;
using System.IO;
using DirectCareConnect.Common.Models.ComponentCommunication;
using DirectCareConnect.Common.XPlat;

[assembly: Dependency(typeof(XamarinBridge))]
namespace DirectCareConnect.Services
{
    public class XamarinBridge : IXamarinBridge
    {
        
        public XamarinBridge()
        {
           
            
        }

        public async Task<List<string>> DisplayAlert(string title, string msg, string cancel)
        {
            await App.Current.MainPage.DisplayAlert(title, msg, cancel);

            List<string> result = new List<string>()
            {
                "Lorem",
                "Ipsum",
                "Dolorem",
            };

            return result;
        }

        async public Task<FilePickerResult> LaunchFilePicker()
        {
            try
            {
                FileData fileData = await CrossFilePicker.Current.PickFile();
                if (fileData == null)
                    return null; // user canceled file picking

                Guid newId = Guid.NewGuid();
                string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), newId.ToString() + "-" + fileData.FileName);
                File.WriteAllBytes(fileName, fileData.DataArray);

                return new FilePickerResult
                {
                     FileId=newId,
                     FileLength=fileData.DataArray.Length,
                     OriginalFileName=fileData.FileName,
                     PathToUpload=fileName
                };
            }
            catch
            {
                
            }

            return null;
        }

    }
}
