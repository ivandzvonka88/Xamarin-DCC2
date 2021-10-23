
using Blazored.Toast.Services;
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
using System.Linq;
using System.Threading.Tasks;

namespace DirectCareConnect.Blazor.Pages.Bases
{
    public abstract class EndSessionBase: ModelBase
    {
        private EndSessionModelBase _model;
        [Inject] protected NavigationManager navigationManager { get; set; }
        [Inject] protected IDatabaseService DbService { get; set; }
        [Inject] protected IJobService JobService { get; set; }
        [Inject] protected IJSRuntime JsRuntime { get; set; }
        [Inject] protected IRestClient RestClient { get; set; }
        [Inject] protected IToastService ToastService { get; set; }

        [Parameter]
        public int ClientId { get; set; }

        [Parameter]
        public int ClientServiceId { get; set; }

        [Parameter]
        public int RemoteDocumentId { get; set; }

        [Parameter]
        public int InternalDocId { get; set; }

        protected virtual EndSessionModelBase model {
            get
            {
                return this._model;
            }

            set
            {
                if (this._model != null)
                    this._model.Updated -= _model_Updated;

                this._model = value;
                this._model.Updated += _model_Updated;
            }
        }

        private void _model_Updated(object sender, EventArgs e)
        {
            ResolutionValid();
            InputReceived();
        }

        async public virtual Task Save()
        {
            this.Log("Save clicked");
            this.model.SetLoading(nameof(Save), true);
            this.StateHasChanged();
            
            if (!ResolutionValid())
            {
                this.Log("Not valid");
                this.model.SetLoading(nameof(Save), false);
                this.StateHasChanged();
                return;
            }

            this.Log("valid");
            string jsonModel=await SetCommonModelFields();
            this.Log("common");


            bool success=await this.DbService.SaveDirtyNote(this.ClientId, this.ClientServiceId, this.model.DocId, this.model.RemoteDocId, jsonModel);
            if (!success)
            {
                this.Log("no success");
                this.model.HasErrors = true;
                this.ToastService.ShowError("An error has occurred while saving this note, please try again.");
                this.model.SetLoading(nameof(Save), false);
                this.StateHasChanged();
                return;
            }

            this.Log("success");
            //send to api as incomplete
            var updated = await this.RestClient.UpdateWebServer();
            if (!updated)
            {
                this.model.HasErrors = true;
                //this.model.ErrorMessage = "An error has occurred, please try again.";
                this.ToastService.ShowError("This note has been saved, however an error has occurred while updating the server, please try again later.");
                this.model.SetLoading(nameof(Save), false);
                this.StateHasChanged();
                return;
            }


            var submission = await this.SetNote();
            if (submission == null)
            {
                this.ToastService.ShowError("This note has been saved, however an error has occurred while updating the server while saving note, please try again later.");
                this.model.SetLoading(nameof(Save), false);
                this.StateHasChanged();
                return;
            }
            else if (submission.Er.Code == 0 && submission.PendingDocumentations != null)
            {
                await this.DbService.UpdatePendingDocumentation(submission.PendingDocumentations);
            }

            this.model.SetLoading(nameof(Save), false);
            this.StateHasChanged();
            ToastService.ShowSuccess("Note saved");

        }

        async private Task<string> SetCommonModelFields()
        {
            this.model.IsSaved = true;
            this.model.HasErrors = false;
            this.model.ErrorMessage = "";


            if (IsNoteValid() && IsModelValid())
            {
                this.model.IsValid = true;
            }

            if (!this.model.NoteDate.HasValue)
            {
                this.model.NoteDate = DateTime.UtcNow;
            }
           var jsonModel= this.model.ToString();

            if (!this.model.IsDesigneeLocked && this.model.SelectedDesignee != String.Empty)
            {
                var loc = await this.JobService.GetCurrentLocation();
                if (loc != null)
                {
                    this.model.DesigneeLat = loc.Latitude;
                    this.model.DesigneeLon = loc.Longitude;
                    var location = (await this.JobService.GetClosestLocations()).FirstOrDefault();
                    if (location != null)
                    {
                        this.model.DesigneeLocationId = location.ClientLocationId;
                        this.model.DesigneeLocationTypeId = location.LocationTypeId;
                    }
                    else
                    {
                        this.model.DesigneeLocationId = 0;
                        this.model.DesigneeLocationTypeId = 0;
                    }
                }
                else
                {
                    this.model.DesigneeLat = 0;
                    this.model.DesigneeLon = 0;
                    this.model.DesigneeLocationId = 0;
                    this.model.DesigneeLocationTypeId = 0;
                }
            }

            return jsonModel;
        }

        async public virtual  Task Submit()
        {
            this.model.SetLoading(nameof(Submit), true);
            this.model.HasErrors = false;
            this.model.ErrorMessage = "";
            this.model.Completed = true;

            var updated = await this.RestClient.UpdateWebServer();
            
            if (!updated)
            {
                this.model.HasErrors = true;
                this.ToastService.ShowError("An error has occurred, please try again.","");
                this.model.SetLoading(nameof(Submit), false);
                return;
            }

            var submission = await this.SetNote();

            if (submission == null)
            {
                this.ToastService.ShowError("An error has occurred, please try again.", "");
                this.model.SetLoading(nameof(Submit), false);
                this.StateHasChanged();
                return;
            }
            else if (submission.Er.Code == 0 && submission.PendingDocumentations != null)
            {
            
                await this.JobService.ClosePendingDocumentation(this.ClientId, this.ClientServiceId, this.RemoteDocumentId);
            
                await this.DbService.UpdatePendingDocumentation(submission.PendingDocumentations);
                BlazorMobile.Common.Services.BlazorMobileService.PostMessage<string>("myNotification", "update dashboard");
            }

            this.model.SetLoading(nameof(Submit), false);
            ToastService.ShowSuccess("Note submitted");
            GoBack();
        }

        protected abstract Task<SetNoteResponse> SetNote();
        protected abstract bool IsModelValid();

        protected ElementReference inputTypeFileElement;

        protected string RestToken { get; set; }

        async protected override Task OnInitializedAsync()
        {
            this.RestToken = (await this.DbService.GetCurrentCredentialsAsync()).Token;
            if(this.RestToken=="" || this.RestToken==null)
            {
                navigationManager.NavigateTo("/login");
                return;
            }
            await base.OnInitializedAsync();

            this.model.IsSaved = true;
            this.model.HasErrors = false;

        }
        public void GoBack()
        {
            navigationManager.NavigateTo("/dashboard");
        }


        public virtual void InputReceived()
        {
            this.model.IsSaved = false;
            this.model.IsValid = false;
        }

        public virtual void DropDownChanged(object sender, EventArgs e)
        {
            InputReceived();
        }


        public async Task AttachFile()
        {
            try
            {
                var x = await XamarinBridge.LaunchFilePicker();
                string msg = String.Empty;
                if (x == null)
                {

                }
                else
                {

                }

                this.model.FileName = x.OriginalFileName;
                this.model.FullAttachmentPath = x.PathToUpload;
                this.model.FileSize = x.FileLength;
                this.model.FileId = x.FileId;
                this.StateHasChanged();
                
            }

            catch
            {
                
            }
        }

        protected virtual bool ResolutionValid()
        {
            this.model.ErrorMessage = "";
            if (!this.model.IsDesigneeLocked && this.model.Resolution == Common.Constants.NoteResolution.DesigneeSign)
            {
                if (this.model.SelectedDesignee == String.Empty)
                {
                    this.model.HasErrors = true;
                    this.model.IsValid = false;
                    this.model.ErrorMessage = "Please select a designee";
                    return false;
                }

                if (this.model.DesigneePin.IsNullOrEmpty())
                {
                    this.model.HasErrors = true;
                    this.model.IsValid = false;
                    this.model.ErrorMessage = "Designee must enter pin";
                    return false;
                }

                if (this.model.DesigneePin != this.model.Designees.Where(p => p.UniqueDesigneeId == this.model.SelectedDesignee).FirstOrDefault()?.Pin)
                {
                    this.model.HasErrors = true;
                    this.model.IsValid = false;
                    this.model.ErrorMessage = "Invalid designee pin";
                    return false;
                }
            }

            return true;
        }

        protected virtual bool IsNoteValid()
        {
            if (this.model.Note.IsNullOrEmpty())
            {
                this.model.HasErrors = true;
                this.model.IsValid = false;
                this.model.ErrorMessage = "Please enter a note";
                return false;
            }
            return true;
        }

        
    }
}
