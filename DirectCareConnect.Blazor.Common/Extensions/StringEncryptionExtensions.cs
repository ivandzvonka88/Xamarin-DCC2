using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace DirectCareConnect.Common.Extensions
{
    public static class StringEncryptionExtensions
    {
        public static string OneWayHash(this string value)
        {
            using (var sha = SHA512.Create())
            {
                return Convert.ToBase64String(sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(value)));
            }
        }
    }
}
