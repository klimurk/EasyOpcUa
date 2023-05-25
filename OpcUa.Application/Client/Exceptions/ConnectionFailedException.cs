using System.Runtime.Serialization;

namespace OpcUa.Application.Client.Exceptions
{
    [Serializable]
    internal class ConnectionFailedException : Exception
    {
        public ConnectionFailedException()
        {
        }

        public ConnectionFailedException(string? message) : base(message)
        {
        }

        public ConnectionFailedException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected ConnectionFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}