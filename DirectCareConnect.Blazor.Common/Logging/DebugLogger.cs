using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.Common.Logging
{
    public static class DebugLogger
    {
        private static List<string> Logs = new List<string>();

        public static void AddLog(string message)
        {
            Logs.Add(message);
        }

        public static string GetLogs()
        {
            string str = String.Empty;
            foreach(var log in Logs)
            {
                str += " | " + log;
            }

            return str;
        }
    }
}
