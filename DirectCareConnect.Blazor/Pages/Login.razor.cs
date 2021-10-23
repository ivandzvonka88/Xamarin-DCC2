using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DirectCareConnect.Common.Models.Login;
using DirectCareConnect.Common.Interfaces;
using DirectCareConnect.Common.Interfaces.Authentication;
using Microsoft.JSInterop;
using DirectCareConnect.Blazor.Components;
using Microsoft.Extensions.Logging;
using DirectCareConnect.Common.Models.Global;
using Blazored.Toast.Services;
using DirectCareConnect.Blazor.Pages.Bases;
using DirectCareConnect.Common.Extensions;

namespace DirectCareConnect.Blazor.Pages
{
    public class LoginBase: ModelBase
    {
        
        [Inject] IJSRuntime JsRuntime { get; set; }
        [Inject] ILoginService LoginService{ get; set; }
        [Inject] IModalService ModalService { get; set; }

        [Inject] IToastService toastService { get; set; }

        protected LoginModel model = new LoginModel();
        protected string message = String.Empty;

        async protected override Task OnInitializedAsync()
        {
            try
            {
                this.SetPageLoading(true, "Login");
                this.Log("Logging in");
                var login = await this.LoginService.CheckLogin();
                this.Log("Logging in done");
                if (login != null)
                {
                    if (login.Success)
                    {
                        this.SetPageLoading(false);
                        StateHasChanged();
                        NavigationManager.NavigateTo("dashboard");
                    }
                    else
                    {
                        if (this.model.Email.IsNullOrEmpty())
                            this.model = new LoginModel() { Email = login.EmailAddress };
                    }

                }
            }

            catch(Exception ee)
            {
                this.Log(ee.Message);
            }

            finally
            {
                this.SetPageLoading(false);
            }
        }

        
        async public void TryLogin()
        {
           // ModalService.Show("test", "one");
           // await JsRuntime.InvokeAsync<object>("ShowAlert", "Test");
          //  return;
            this.SetLoading("Login",true);
            Result loginResult = await LoginService.Login(model.Email, model.Password);
            this.SetLoading("Login", false);

            if (!loginResult.Success)
            {
                if (loginResult.Exception.IsNullOrEmpty())
                {
                    model.CredentialsInvalid = true;
                    this.StateHasChanged();
                }
                else
                {
                    toastService.ShowError(loginResult.Exception,"An error has occurred");
                }
            }
            else
            {
                NavigationManager.NavigateTo("dashboard");
            }

        }
    }
}
