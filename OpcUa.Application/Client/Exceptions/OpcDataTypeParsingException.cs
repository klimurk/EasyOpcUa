using System.Runtime.Serialization;

namespace OpcUa.Application.Client.Exceptions;

[Serializable]
internal class OpcDataTypeParsingException : Exception
{
    public OpcDataTypeParsingException()
    {
    }

    public OpcDataTypeParsingException(string? message) : base(message)
    {
    }

    public OpcDataTypeParsingException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected OpcDataTypeParsingException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}