using System.Runtime.Serialization;

namespace OpcUa.Persistance.Exceptions.Writer
{
	[Serializable]
	internal class OpcWriteException : Exception
	{
		public OpcWriteException()
		{
		}

		public OpcWriteException(string? message) : base(message)
		{
		}

		public OpcWriteException(string? message, Exception? innerException) : base(message, innerException)
		{
		}

		protected OpcWriteException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}