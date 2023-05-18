using System.Runtime.Serialization;

namespace OpcUa.Application.Client.Exceptions
{
    [Serializable]
    internal class BrowsingException : Exception
    {
        public BrowsingException()
        {
        }

        public BrowsingException(string? message) : base(message)
        {
        }

        public BrowsingException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected BrowsingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}