using OpcUa.Domain;
using OpcUa.Domain.Basics;
using OpcUa.Persistance.Exceptions.Related;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace OpcUa.Persistance.Helpers;

public static class OpcXmlParser
{
	private const string structDefinitionString = "tns:";
	/// <summary>Parses a XML string for the a default data type</summary>
	/// <param name="xmlStringToParse">The XML string containing data type information</param>
	/// <param name="stringToParserFor">The data type as string to search for</param>
	/// <returns>The created objects after parsing for default data type</returns>
	/// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
	public static IEnumerable<OpcNode> ParseOpcStructs(string xmlStringToParse, string stringToParserFor)
	{
		IEnumerable<OpcNode> result;

		//Remove last XML sign and create a XML document out of the dictionary string
		if (xmlStringToParse.EndsWith("\n"))
		{
			xmlStringToParse = xmlStringToParse.Remove(xmlStringToParse.Length - 1);
		}
		XmlDocument docToParse = new();
		docToParse.LoadXml(xmlStringToParse);

		//Get a XML node list of objectes named by "stringToParseFor"
		docToParse.GetElementsByTagName(stringToParserFor, stringToParserFor);

		XmlNodeList nodeList = docToParse.GetElementsByTagName("opc:StructuredType");
		result = ExtractInnerNodes(nodeList, stringToParserFor);

		//Check if result contains another struct/UDT inside and parse for var name and var type
		//Note: This check is consistent even if there are more structs/UDTs inside of structs/UDTs

		return result;
	}

	private static IEnumerable<OpcNode> ExtractInnerNodes(XmlNodeList nodeList, string stringToParserFor)
	{
		List<OpcNode> result = new();
		XmlNode foundNode = null;

		//search for the attribute name == "stringToParseFor"
		foreach (XmlNode node in nodeList)
		{
			if (node.Attributes["Name"].Value == stringToParserFor)
			{
				foundNode = node;
				break;
			}
		}

		//check if attribute name was found
		if (foundNode == null)
		{
			return null;
		}

		//get child nodes of parent node with attribute name == "stringToParseFor" and parse for var name and var type
		foreach (XmlNode xmlNode in foundNode.ChildNodes)
		{
			string? name = xmlNode.Attributes["Name"]?.Value;
			string? typeName = xmlNode.Attributes["TypeName"]?.Value;
			if (typeName is null || name is null) continue;
			OpcNode node = new()
			{
				Name = name
			};
			bool isStruct = typeName.Contains(structDefinitionString);
			typeName = typeName.Remove(0, structDefinitionString.Length);
			node.DataType = typeName;
			if (isStruct)
			{
				node.InnerNodes = ExtractInnerNodes(nodeList, typeName);
			}
			result.Add(node);
		}
		return result;
	}

	public static IEnumerable<OpcNode> InsertValuesInStruct(this IEnumerable<OpcNode> varList, byte[] byteResult)
	{
		//Byte decoding index
		int index = 0;

		//Int used to decode arrays
		int arraylength = 0;

		//Start decoding for opc data types
		foreach (OpcNode node in varList)
		{
			foreach (OpcNode val in node.InnerNodes)
			{
				val.Value = SetStructureNodeValue(ref index, ref arraylength, val.Name, val.DataType, byteResult);
			}
		}
		return varList;
	}
	private static dynamic SetStructureNodeValue(ref int index, ref int arrayLength, string TagName, string dataType, byte[] byteResult)
	{
		dynamic result = new object();
		switch (dataType)
		{
			case OpcConstants.TypeBoolean when arrayLength <= 0:
				{
					result = BitConverter.ToBoolean(byteResult, index);
					index++;
					break;
				}
			case OpcConstants.TypeByte when arrayLength <= 0:
				{
					result = byteResult[index];
					index++;
					break;
				}
			case OpcConstants.TypeByte when arrayLength > 0:
				{
					int[] tempArray = new int[arrayLength];
					for (int i = 0; i < arrayLength; i++)
					{
						tempArray[i] = byteResult[index];
						index++;
					}
					result = tempArray;
					arrayLength = 0;
					break;
				}
			case OpcConstants.TypeInt16 when arrayLength <= 0:
				{
					result = BitConverter.ToInt16(byteResult, index);
					index += 2;
					break;
				}
			case OpcConstants.TypeInt16 when arrayLength > 0:
				{
					short[] tempArray = new short[arrayLength];
					for (int i = 0; i < arrayLength; i++)
					{
						tempArray[i] = BitConverter.ToInt16(byteResult, index);
						index += 2;
					}
					result = tempArray;
					arrayLength = 0;
					break;
				}
			case OpcConstants.TypeInt32 when arrayLength > 0:
				{
					int[] tempArray = new int[arrayLength];
					for (int i = 0; i < arrayLength; i++)
					{
						tempArray[i] = BitConverter.ToInt32(byteResult, index);
						index += 4;
					}
					result = tempArray;
					arrayLength = 0;
					break;
				}
			case OpcConstants.TypeInt32 when arrayLength <= 0 && !TagName.Contains("_Size"):
				{
					result = BitConverter.ToInt32(byteResult, index);
					index += 4;
					break;
				}
			case OpcConstants.TypeInt32 when TagName.Contains("_Size"):
				{
					arrayLength = BitConverter.ToInt32(byteResult, index);
					result = arrayLength;
					index += 4;
					break;
				}
			case OpcConstants.TypeInt64 when arrayLength <= 0:
				{
					result = BitConverter.ToInt64(byteResult, index);
					index += 8;
					break;
				}
			case OpcConstants.TypeInt64 when arrayLength > 0:
				{
					long[] tempArray = new long[arrayLength];
					for (int i = 0; i < arrayLength; i++)
					{
						tempArray[i] = BitConverter.ToInt64(byteResult, index);
						index += 8;
					}
					result = tempArray;
					arrayLength = 0;
					break;
				}
			case OpcConstants.TypeUInt16 when arrayLength > 0:
				{
					ushort[] tempArray = new ushort[arrayLength];
					for (int i = 0; i < arrayLength; i++)
					{
						tempArray[i] = BitConverter.ToUInt16(byteResult, index);
						index += 2;
					}
					result = tempArray;
					arrayLength = 0;
					break;
				}
			case OpcConstants.TypeUInt16 when arrayLength <= 0:
				{
					result = BitConverter.ToUInt16(byteResult, index);
					index += 2;
					break;
				}
			case OpcConstants.TypeUInt32 when arrayLength > 0:
				{
					uint[] tempArray = new uint[arrayLength];
					for (int i = 0; i < arrayLength; i++)
					{
						tempArray[i] = BitConverter.ToUInt32(byteResult, index);
						index += 4;
					}
					result = tempArray;
					arrayLength = 0;
					break;
				}
			case OpcConstants.TypeUInt32 when arrayLength <= 0:
				{
					result = BitConverter.ToUInt32(byteResult, index);
					index += 4;
					break;
				}
			case OpcConstants.TypeUInt64 when arrayLength > 0:
				{
					ulong[] tempArray = new ulong[arrayLength];
					for (int i = 0; i < arrayLength; i++)
					{
						tempArray[i] = BitConverter.ToUInt64(byteResult, index);
						index += 8;
					}
					result = tempArray;
					arrayLength = 0;
					break;
				}
			case OpcConstants.TypeUInt64 when arrayLength <= 0:
				{
					result = BitConverter.ToUInt64(byteResult, index);
					index += 8;
					break;
				}
			case OpcConstants.TypeFloat when arrayLength <= 0:
				{
					result = BitConverter.ToSingle(byteResult, index);
					index += 4;
					break;
				}
			case OpcConstants.TypeFloat when arrayLength > 0:
				{
					float[] tempArray = new float[arrayLength];
					for (int i = 0; i < arrayLength; i++)
					{
						tempArray[i] = BitConverter.ToSingle(byteResult, index);
						index += 4;
					}
					result = tempArray;
					arrayLength = 0;
					break;
				}
			case OpcConstants.TypeDouble when arrayLength <= 0:
				{
					result = BitConverter.ToDouble(byteResult, index);
					index += 8;
					break;
				}
			case OpcConstants.TypeDouble when arrayLength > 0:
				{
					double[] tempArray = new double[arrayLength];
					for (int i = 0; i < arrayLength; i++)
					{
						tempArray[i] = BitConverter.ToDouble(byteResult, index);
						index += 8;
					}
					result = tempArray;
					arrayLength = 0;
					break;
				}
			case OpcConstants.TypeString when arrayLength <= 0:
				{
					int stringLength = BitConverter.ToInt32(byteResult, index);
					index += 4;
					if (stringLength > 0)
					{
						result = Encoding.UTF8.GetString(byteResult, index, stringLength);
						index += stringLength;
					}
					break;
				}
			case OpcConstants.TypeString when arrayLength > 0:
				{
					for (int i = 0; i < arrayLength; i++)
					{
						int stringLength = BitConverter.ToInt32(byteResult, index);
						index += 4;
						if (stringLength > 0)
						{
							result = Encoding.UTF8.GetString(byteResult, index, stringLength);
							index += stringLength;
						}
					}
					arrayLength = 0;
					break;
				}
			case OpcConstants.TypeCharArray:
				{
					int stringLength = BitConverter.ToInt32(byteResult, index);
					index += 4;
					if (stringLength > 0)
					{
						result = Encoding.UTF8.GetString(byteResult, index, stringLength);
						index += stringLength;
					}
					break;
				}
			case string a when a.Contains(OpcConstants.TypeStruct) && arrayLength <= 0:
				{
					result = OpcConstants.TypeStruct;
					break;
				}
			default:
				throw new OpcDataTypeParsingException($"Data type is too complex to be parsed.{Environment.NewLine} Please avoid array of UDT/Struct inside UDT/Struct");
		}
		return result;
	}
}
