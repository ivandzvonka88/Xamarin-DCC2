using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.Common.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string value)
        {
            return String.IsNullOrEmpty(value);
        }
    }
}
