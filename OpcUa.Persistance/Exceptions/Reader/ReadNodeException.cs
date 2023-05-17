using System.Runtime.Serialization;

namespace OpcUa.Persistance.Exceptions.Reader
{
    [Serializable]
    internal class ReadNodeException : Exception
    {
        public ReadNodeException()
        {
        }

        public ReadNodeException(string? message) : base(message)
        {
        }

        public ReadNodeException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected ReadNodeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}