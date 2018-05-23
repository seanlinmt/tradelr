using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tradelr.Models.activity;

namespace tradelr.Libraries.reporting
{
    public class ErrorMessageType : IActivityMessageType
    {
        public static ErrorMessageType SESSION_EXPIRED = new ErrorMessageType("Session expired for userid {0}");

        private ErrorMessageType(string msg)
            :base(msg,true)
        {
        }

        public override string GetMessageType()
        {
            return "";
        }
    }
}
