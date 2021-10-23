using DirectCareConnect.Blazor.Pages.Bases;
using DirectCareConnect.Common.Impl.Communication.DDezModels;
using DirectCareConnect.Common.Models.UI;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DirectCareConnect.Blazor.Pages
{
    public class EndSessionAttendantCareBase: EndSessionBase
    {
        public EndSessionAttendantCareModel Model
        {
            get
            {
                return this.model as EndSessionAttendantCareModel;
            }

            set
            {
                this.model = value;
            }
        }

        public EndSessionAttendantCareBase()
        {
            this.Model = new EndSessionAttendantCareModel();
        }

        
        async protected override Task OnInitializedAsync()
        {
            try
            {

                this.Model = await this.JobService.GetSessionAttendantCareModel(this.RestToken, this.ClientId, this.ClientServiceId, this.RemoteDocumentId, this.InternalDocId);
                await base.OnInitializedAsync();

                if (ResolutionValid() && IsNoteValid() && IsModelValid())
                {
                    this.Model.IsValid = true;
                }

                await base.OnInitializedAsync();
            }
            catch
            {
                
            }
        }

        protected override bool IsModelValid()
        {
            foreach (var item in this.Model.CareAreas)
            {
                if (item.score == String.Empty)
                {
                    this.model.HasErrors = true;
                    this.model.IsValid = false;
                    this.model.ErrorMessage = "Please fill out scores";
                    return false;
                }
            }

            return true;
        }

        async protected override Task<SetNoteResponse> SetNote()
        {
            return await this.JobService.SetNote(this.RestToken, this.Model);
        }
    }
}
