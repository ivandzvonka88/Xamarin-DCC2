using DirectCareConnect.Blazor.Pages.Bases;
using DirectCareConnect.Common.Extensions;
using DirectCareConnect.Common.Impl.Communication.DDezModels;
using DirectCareConnect.Common.Interfaces;
using DirectCareConnect.Common.Interfaces.Communication;
using DirectCareConnect.Common.Interfaces.Storage;
using DirectCareConnect.Common.Models.Rest;
using DirectCareConnect.Common.Models.UI;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Unosquare.Swan;

namespace DirectCareConnect.Blazor.Pages
{
    
    public class EndSessionRespiteBase: EndSessionBase
    {
        
        public EndSessionRespiteModel Model {
            get
            {
                return this.model as EndSessionRespiteModel;
            }

            set
            {
                this.model = value;
            }
        }

        public EndSessionRespiteBase()
        {
            this.Model = new EndSessionRespiteModel();
            
        }
        async protected override Task OnInitializedAsync()
        {
            try
            {
                this.Model = await this.JobService.GetEndSessionRespiteModel(this.RestToken, this.ClientId, this.ClientServiceId, this.RemoteDocumentId, this.InternalDocId);
                await base.OnInitializedAsync();

                if (ResolutionValid() && IsNoteValid())
                {
                    this.Model.IsValid = true;
                }
            }

            catch
            {
                
            }
        }


        async protected override Task<SetNoteResponse> SetNote()
        {
            return await this.JobService.SetNote(this.RestToken, this.Model);
        }

        protected override bool IsModelValid()
        {
            return true;
        }
    }
}
