using System.Runtime.Serialization;

namespace OpcUa.Persistance.Exceptions.Reader
{
    [Serializable]
    internal class WrongReferencesException : Exception
    {
        public WrongReferencesException()
        {
        }

        public WrongReferencesException(string? message) : base(message)
        {
        }

        public WrongReferencesException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected WrongReferencesException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}