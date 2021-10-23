using Blazored.Toast.Services;
using DirectCareConnect.Blazor.Pages.Bases;
using DirectCareConnect.Common.Extensions;
using DirectCareConnect.Common.Impl.Communication.DDezModels;
using DirectCareConnect.Common.Interfaces.Authentication;
using DirectCareConnect.Common.Models.Global;
using DirectCareConnect.Common.Models.Login;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DirectCareConnect.Blazor.Pages
{
    public class ForgotPasswordBase: ModelBase
    {

        
        protected ForgotPasswordModel model = new ForgotPasswordModel();
        protected string message = String.Empty;
        protected ResetStep Step;
        ForgotPasswordContactModel loginResult;
        [Inject] ILoginService LoginService { get; set; }
        [Inject] IToastService toastService { get; set; }

        public ForgotPasswordBase()
        {
            RequestDisabled = true;
            CodeDisabled = true;
            ConfirmDisabled = true;
        }

        public bool RequestDisabled { get; set; }
        public bool CodeDisabled { get; set; }
        public bool ConfirmDisabled { get; set; }
        public string SendToEmailText { get; set; }
        public string SendToTextText { get; set; }
        async public void TryReset()
        {
            this.SetLoading(nameof(TryReset), true);
            this.loginResult = await LoginService.RequestPasswordReset(model.Email);
            this.SetLoading(nameof(TryReset), false);
            message = "";
            if (!loginResult.Success)
            {
                if (loginResult.Error.IsNullOrEmpty())
                {
                    message = "Invalid Credentials";
                    this.StateHasChanged();
                }
                else
                {
                    toastService.ShowError(loginResult.Error, "An error has occurred");
                }
            }
            else
            {
                this.Step = ResetStep.Contact;
                this.SendToEmailText = "Send email to: " + loginResult.Email;
                this.SendToTextText = "Send text to: " + loginResult.Phone;
                this.StateHasChanged();
            }

        }
        async public void TryCode()
        {
            this.SetLoading(nameof(TryCode), true);
            Result result = await LoginService.ConfirmPasswordResetCode(this.loginResult.UserId,this.model.Code);
            this.SetLoading(nameof(TryCode), false);
            if (result.Success)
            {
                this.Step = ResetStep.Confirm;
                this.StateHasChanged();
            }
            else
            {
                toastService.ShowError(result.Exception, "An error has occurred");
            }
           
        }
        async public void TryConfirm()
        {
            this.SetLoading(nameof(TryConfirm), true);
            Result result = await LoginService.ConfirmPasswordReset(this.loginResult.UserId, this.model.Code, this.model.Password);
            this.SetLoading(nameof(TryConfirm), false);
            if (result.Success)
            {
                this.Step = ResetStep.Done;
                this.StateHasChanged();
            }
            else
            {
                toastService.ShowError(result.Exception, "An error has occurred");
            }
            

        }

        public void GoBack()
        {
            NavigationManager.NavigateTo("/login");
        }

        
        async public void SendToEmail()
        {
            this.SetLoading(nameof(SendToEmail), true);
            Result result = await LoginService.SendPasswordReset(this.loginResult.UserId, "ByEmail");
            this.SetLoading(nameof(SendToEmail), false);
            if (result.Success)
            {
                this.Step = ResetStep.Code;
                this.StateHasChanged();
            }
            else
            {
                toastService.ShowError(result.Exception, "An error has occurred");
            }
        }
        async public void SendToText()
        {
            this.SetLoading(nameof(SendToText), true);
            Result result = await LoginService.SendPasswordReset(this.loginResult.UserId,"ByText");
            this.SetLoading(nameof(SendToText), false);
            if (result.Success)
            {
                this.Step = ResetStep.Code;
                this.StateHasChanged();
            }
            else
            {
                toastService.ShowError(result.Exception, "An error has occurred");
            }
        }

        public void InputReceived()
        {
            this.RequestDisabled = model.Email.IsNullOrEmpty();
            this.CodeDisabled = model.Code.IsNullOrEmpty();
            if(this.model.Password.IsNullOrEmpty() || this.model.ConfirmPassword.IsNullOrEmpty())
            {
                this.ConfirmDisabled = true;
            }
            else
            {
                this.ConfirmDisabled = (this.model.Password != this.model.ConfirmPassword);
            }
        }
        public enum ResetStep
        {
            Email, Contact, Code, Confirm, Done
        }

    }
}
