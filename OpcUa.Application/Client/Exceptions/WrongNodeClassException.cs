using System.Runtime.Serialization;

namespace OpcUa.Application.Client.Exceptions
{
    [Serializable]
    internal class WrongNodeClassException : Exception
    {
        public WrongNodeClassException()
        {
        }

        public WrongNodeClassException(string? message) : base(message)
        {
        }

        public WrongNodeClassException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected WrongNodeClassException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}