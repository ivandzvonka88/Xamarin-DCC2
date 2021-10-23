using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.Common.Models.Global
{
    public class Result
    {
        public Result() { }
        public Result(bool success):this()
        {
            this.Success = success;
        }
        public bool Success { get; set; }
        public object ReturnId { get; set; }
        public string Exception { get; set; }
    }
}
