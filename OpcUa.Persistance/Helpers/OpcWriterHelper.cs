using OpcUa.Domain;
using OpcUa.Domain.Basics;
using OpcUa.Persistance.Exceptions.Related;

namespace OpcUa.Persistance.Helpers;

public static class OpcWriterHelper
{
	/// <summary>Parses a byte array to objects containing tag names and tag data types</summary>
	/// <param name="dataToWrite">The data to analyze</param>
	/// <returns>The length</returns>
	/// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
	public static long GetLengthOfDataToWrite(OpcNode dataToWrite)
	{

		length = GetLengthOfData(dataToWrite);
		return length;
	}

	private static long GetLengthOfData(OpcNode dataToWrite)
	{
		long length = 0;
		foreach (var val in dataToWrite.InnerNodes)
		{
			if (val.InnerNodes.Any())
			{
				length += GetLengthOfData(val);
				continue;
			}
			switch (val.DataType)
			{
				case OpcConstants.TypeBoolean:
				case OpcConstants.TypeByte:
					length++;
					break;
				case OpcConstants.TypeInt16:
				case OpcConstants.TypeUInt16:
					length += 2;
					break;
				case OpcConstants.TypeFloat:
				case OpcConstants.TypeInt32 when !val.Name.Contains("_Size"):
				case OpcConstants.TypeUInt32:
					length += 4;
					break;
				case OpcConstants.TypeDouble:
				case OpcConstants.TypeInt64:
				case OpcConstants.TypeUInt64:
					length += 8;
					break;
				case OpcConstants.TypeString:
					length += ((string)val.Value).Length + 4;
					break;
				case OpcConstants.TypeCharArray:
					length += ((char[])val.Value).Length + 4;
					break;
				default:
					throw new InvalidOpcValueException($"Unknown data type {val.Name} '{val.DataType}'. Can't determine length of data");
			}


		}
		return length;
	}
}