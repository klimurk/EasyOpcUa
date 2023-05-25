using System.Runtime.Serialization;

namespace OpcUa.Application.Client.Exceptions
{
    [Serializable]
    internal class InvalidOpcValueException : Exception
    {
        public InvalidOpcValueException()
        {
        }

        public InvalidOpcValueException(string? message) : base(message)
        {
        }

        public InvalidOpcValueException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected InvalidOpcValueException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}