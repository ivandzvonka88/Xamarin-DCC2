using DirectCareConnect.Common.Extensions;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DirectCareConnect.Common.Models.Login
{
    public class LoginModel
    {
        private string email;
        private string password;

        public bool Checking{ get; set; }
        
        public bool CredentialsInvalid { get; set; }
        public bool ButtonDisabled { get; set; }

        private bool Valid
        {
            get
            {
                if (this.Email.IsNullOrEmpty() || this.Password.IsNullOrEmpty())
                    return false;

                return true;
            }
        }
        public LoginModel()
        {
            this.ButtonDisabled = true;
        }

        public string Email
        {
            get
            {
                return this.email;
            }
            set
            {
                this.email = value;
                this.ButtonDisabled = !this.Valid;
            }
        }

        public string Password
        {
            get
            {
                return this.password;
            }
            set
            {
                this.password = value;
                this.ButtonDisabled = !this.Valid;
            }
        }

    }
}
