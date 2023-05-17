using System.Runtime.Serialization;

namespace OpcUa.Persistance.Exceptions.Connection
{
    [Serializable]
    internal class ServersNotFoundException : Exception
    {
        public ServersNotFoundException()
        {
        }

        public ServersNotFoundException(string? message) : base(message)
        {
        }

        public ServersNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected ServersNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}