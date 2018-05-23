using System;

namespace tradelr.Models.activity
{
    public class ActivityMessage 
    {
        public readonly long appid;

        private readonly IActivityMessageType msgType;
        private readonly string[] msgParts;
        public readonly long? targetUserid;

        public ActivityMessage(long appid, long? target, IActivityMessageType type, params string[] parts)
        {
            this.appid = appid;
            targetUserid = target;
            msgType = type;
            msgParts = new string[parts.Length];

            if (type == null)
            {
                throw new NullReferenceException();
            }

            if (parts.Length != type.paramCount)
            {
                throw new ArgumentException(
                    parts.Length + " != " + type.paramCount);
            }

            for (int i = parts.Length; --i >= 0; )
            {
                string p = parts[i];
                if (p == null)
                {
                    throw new NullReferenceException(i.ToString());
                }
                msgParts[i] = p;
            }
        }

        public bool AllowMultiple()
        {
            return msgType.allowMultiple;
        }

        public string GetMessageType()
        {
            return msgType.GetMessageType();
        }

        public string Format()
        {
            return string.Format(msgType.formatString, msgParts);
        }
    }
}