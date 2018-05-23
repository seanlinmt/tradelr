using clearpixels.Logging;
using System;
using System.Collections.Specialized;
using tradelr.Library;
using Syslog = clearpixels.Logging.Syslog;
namespace Etsy
{
    public class Parameters
    {
        public NameValueCollection parameters { get; private set; }

        public Parameters()
        {
            parameters = new NameValueCollection();
        }

        public void AddParameter(string name, string value)
        {
            if (value != null)
            {
                try
                {
                    parameters.Add(name, value);
                }
                catch (Exception ex)
                {
                    Syslog.Write(ex.Message + string.Format(": {0} {1}", name, value));
                    throw;
                }

            }
        }

        public string ToQueryString()
        {
            return parameters.ToQueryString(true);
        }
    }

}
