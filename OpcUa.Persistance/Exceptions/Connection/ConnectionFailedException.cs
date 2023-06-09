﻿using System.Runtime.Serialization;

namespace OpcUa.Persistance.Exceptions.Connection
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