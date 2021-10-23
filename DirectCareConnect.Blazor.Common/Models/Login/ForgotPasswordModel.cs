using DirectCareConnect.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.Common.Models.Login
{

    public class ForgotPasswordModel
    {
        private string email;
        private string password;
        private string confirmPassword;
        private string code;
        protected bool Valid
        {
            get
            {
                if (this.Email.IsNullOrEmpty())
                    return false;

                return true;
            }
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
                
            }
        }
        public string ConfirmPassword
        {
            get
            {
                return this.confirmPassword;
            }
            set
            {
                this.confirmPassword = value;
                
            }
        }
        public string Code
        {
            get
            {
                return this.code;
            }
            set
            {
                this.code = value;
                
            }
        }
    }
}
