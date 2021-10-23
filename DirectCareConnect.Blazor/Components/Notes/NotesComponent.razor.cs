using DirectCareConnect.Blazor.Pages.Bases;
using DirectCareConnect.Common.Interfaces;
using DirectCareConnect.Common.Models.Rest;
using DirectCareConnect.Common.Models.UI;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DirectCareConnect.Blazor.Components.Notes
{
    public class NotesComponentBase: NotesComponentsBase
    {
        public async Task AttachFile()
        {
            try
            {
                var x = await XamarinBridge.LaunchFilePicker();
                string msg = String.Empty;
                
                this.EndSessionModel.FileName = x.OriginalFileName;
                this.EndSessionModel.FullAttachmentPath = x.PathToUpload;
                this.EndSessionModel.FileSize = x.FileLength;
                this.EndSessionModel.FileId = x.FileId;
                this.StateHasChanged();
            }

            catch
            {
                
            }
        }
    }
}
