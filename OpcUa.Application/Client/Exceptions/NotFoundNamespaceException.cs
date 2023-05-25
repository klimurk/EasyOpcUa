using System.Runtime.Serialization;

namespace OpcUa.Application.Client.Exceptions
{
	[Serializable]
	internal class NotFoundNamespaceException : Exception
	{
		public NotFoundNamespaceException()
		{
		}

		public NotFoundNamespaceException(string? message) : base(message)
		{
		}

		public NotFoundNamespaceException(string? message, Exception? innerException) : base(message, innerException)
		{
		}

		protected NotFoundNamespaceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}