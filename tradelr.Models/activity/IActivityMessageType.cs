using System;

namespace tradelr.Models.activity
{
    public abstract class IActivityMessageType
    {
        /// <summary>
        /// allow multiple messages of this type to appear
        /// </summary>
        public readonly bool allowMultiple;
        public readonly string formatString;
        public readonly int paramCount;
        
        protected IActivityMessageType(string msg,bool multiples)
        {
            allowMultiple = multiples;
            formatString = msg;
            paramCount = GetParamCount(formatString);
        }

        private static int GetParamCount(String formatString)
        {
            int count = 0;
            for (int i = 0, n = formatString.Length; i < n; ++i)
            {
                char ch = formatString[i];
                if ('{' == ch)
                {
                    if (i + 1 < n && '{' != formatString[i + 1])
                    {
                        ++count;
                    }
                    else
                    {
                        ++i;
                    }
                }
            }
            return count;
        }

        public abstract string GetMessageType();
    }
}
