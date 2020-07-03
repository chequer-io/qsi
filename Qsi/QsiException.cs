using System;
using System.Runtime.Serialization;

namespace Qsi
{
    public class QsiException : Exception
    {
        public QsiException()
        {
        }

        protected QsiException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public QsiException(string message) : base(message)
        {
        }

        public QsiException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
