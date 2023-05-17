using System.Runtime.Serialization;

namespace OpcUa.Persistance.Exceptions.Related
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