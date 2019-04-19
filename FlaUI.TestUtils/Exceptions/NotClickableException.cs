using System;
using System.Runtime.Serialization;

namespace UI.TestUtils.Exceptions
{
    [Serializable]
    public class NotClickableException : Exception
    {
        public NotClickableException()
        {
        }

        public NotClickableException(string message) : base(message)
        {
        }

        public NotClickableException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NotClickableException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}