using System.Runtime.Serialization;

namespace OpcUa.Application.Client.Exceptions
{
    [Serializable]
    internal class OpcConnectionException : Exception
    {
        public OpcConnectionException()
        {
        }

        public OpcConnectionException(string? message) : base(message)
        {
        }

        public OpcConnectionException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected OpcConnectionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}