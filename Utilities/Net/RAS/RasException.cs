using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities.Net.RAS
{
    [Serializable]
    public class RasException : Exception
    {
        public UInt32 ErrorCode { get; private set; }

        public RasException(UInt32 errorCode)
        {
            ErrorCode = errorCode;
        }

        public RasException(String message) : base(message) { }

        public RasException(String message, Exception inner) : base(message, inner) { }

        protected RasException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public override String Message
        {
            get
            {
                String message;
                try
                {
                    if (ErrorCode != 0)
                    {
                        StringBuilder sb = new StringBuilder(1024);
                        UInt32 result = RasNativeMethods.RasGetErrorString(ErrorCode, sb, sb.Capacity);
                        if (result > 0)
                        {
                            throw new Exception("Unknown RAS exception.");
                        }
                        message = sb.ToString();
                    }
                    else
                    {
                        message = base.Message;
                    }
                }
                catch (Exception e)
                {
                    message = String.Format("ErrorCode: {0}. {1}", ErrorCode, e.Message);
                }
                return message;
            }
        }
    }
}
