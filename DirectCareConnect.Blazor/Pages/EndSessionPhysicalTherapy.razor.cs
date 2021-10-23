using DirectCareConnect.Blazor.Pages.Bases;
using DirectCareConnect.Common.Impl.Communication.DDezModels;
using DirectCareConnect.Common.Models.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DirectCareConnect.Blazor.Pages
{
    public class EndSessionPhysicalTherapyBase : EndSessionBase
    {
        public EndSessionPhysicalTherapyModel Model
        {
            get
            {
                return this.model as EndSessionPhysicalTherapyModel;
            }

            set
            {
                this.model = value;
            }
        }

        public EndSessionPhysicalTherapyBase()
        {
            this.Model = new EndSessionPhysicalTherapyModel();
        }

        async protected override Task OnInitializedAsync()
        {
            try
            {
                this.Model = await this.JobService.GetEndSessionPhysicalTherapyModel(this.RestToken, this.ClientId, this.ClientServiceId, this.RemoteDocumentId, this.InternalDocId);
                await base.OnInitializedAsync();

                if (ResolutionValid() && IsNoteValid() && IsModelValid())
                {
                    this.Model.IsValid = true;
                }

            }
            catch
            {

            }
        }

        protected override bool IsModelValid()
        {
            foreach (var item in this.Model.LongTermObjectives)
            {
                this.Log("Long term objectives");

                foreach (var shortTerm in item.shortTermGoals)
                {
                    this.Log("short term objectives");
                    if (shortTerm.score == String.Empty)
                    {
                        this.model.HasErrors = true;
                        this.model.IsValid = false;
                        this.model.ErrorMessage = "Please fill out scores";
                        return false;
                    }
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
