using OpcUa.Domain;
using Opc.Ua;
using OpcUa.Applications.Errors;
using System.Runtime.Serialization;

namespace OpcUa.Client.Applications.Helpers;

public static class OpcWriterHelper
{
	/// <summary>Parses a byte array to objects containing tag names and tag data types</summary>
	/// <param name="dataToWrite">The data to analyze</param>
	/// <returns>The length</returns>
	/// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
	public static long GetLengthOfDataToWrite(OpcNode dataToWrite)
	{

		long length = GetLengthOfData(dataToWrite);
		return length;
	}

	private static long GetLengthOfData(OpcNode dataToWrite)
	{
		long length = 0;
		foreach (var node in dataToWrite.InnerNodes)
		{
			if (node.InnerNodes.Any())
			{
				length += GetLengthOfData(node);
				continue;
			}
			switch (node.DataType)
			{
				case BuiltInType.Boolean:
				case BuiltInType.Byte:
					length++;
					break;
				case BuiltInType.Int16:
				case BuiltInType.UInt16:
					length += 2;
					break;
				case BuiltInType.Float:
				case BuiltInType.Int32 when !node.Name.Contains("_Size"):
				case BuiltInType.UInt32:
					length += 4;
					break;
				case BuiltInType.Double:
				case BuiltInType.Int64:
				case BuiltInType.UInt64:
					length += 8;
					break;
				case BuiltInType.String:
					length += ((string)node.Value).Length + 4;
					break;
				//case BuiltInType.CharArray:
				//	length += ((char[])node.Value).Length + 4;
				//	break;
				default:
					throw new InvalidOpcValueException($"Unknown data type {node.Name} '{node.DataType}'. Can't determine length of data");
			}


		}
		return length;
	}
}

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