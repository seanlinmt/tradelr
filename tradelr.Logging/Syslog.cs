using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Security;
using Elmah;

namespace tradelr.Logging
{
    public enum ErrorLevel
    {
        [Description("Critical")]
        CRITICAL,
        [Description("Error")]
        ERROR,
        [Description("Warning")]
        WARNING,
        [Description("Information")]
        INFORMATION,
        [Description("Verbose")]
        VERBOSE
    }
    [SecuritySafeCritical]
    public class Syslog
    {
        //public static void Write(ErrorLevel level, Message msg)
        //{
        //    Write(level, msg.Format());
        //}

        public static void Write(Exception ex)
        {
            ErrorLog.GetDefault(null).Log(new Error(ex));
        }

        public static void Write(ErrorLevel level, string message)
        {
            // show stack
            var t = new StackTrace(true);
            ErrorLog.GetDefault(null).Log(new Error(new Exception(string.Concat(level.ToString(), ":", message, ":", t.ToString()))));
        }
    }
}