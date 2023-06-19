//using AutoMapper;
//using Newtonsoft.Json.Linq;
//using Opc.Ua;
//using OpcUa.Domain;
//using OpcUa.Domain.Contracts.Client;
//using OpcUa.Persistance.Exceptions.Writer;
//using System.Text;
//using System.Xml;

//namespace OpcUa.Persistance.Client;

//public class OpcWriter
//{
//	private readonly IMapper _mapper;

//	public OpcWriter(IMapper mapper) => _mapper = mapper;

//	/// <summary>Writes values to node Ids</summary>
//	/// <param name="value">The values as strings</param>
//	/// <param name="nodeIdString">The node Ids as strings</param>
//	/// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
//	public int WriteValues(IOpcClient client, IList<OpcWriteNodeValue> nodes)
//	{
//		//Create a collection of values to write
//		WriteValueCollection valuesToWrite = new(nodes.Select(_mapper.Map<WriteValue>));
//		StatusCodeCollection result = new();

//		try
//		{
//			//Write the collection to the server
//			client.Session.Write(null, valuesToWrite, out result, out DiagnosticInfoCollection diagnostics);
//		}
//		catch (Exception e)
//		{
//			//handle Exception here
//			throw new OpcWriteException(e.Message, e.InnerException);
//		}
//		StatusCode? errorWritingCode = result.FirstOrDefault(s => s != 0);
//		if (errorWritingCode is not null)
//		{
//			throw new OpcWriteException(errorWritingCode.ToString());
//		}
//		return result.Count;

//	}
//	public async Task<int> WriteValuesAsync(IOpcClient client, IList<OpcWriteNodeValue> nodes, CancellationToken cancellationToken)
//	{
//		//Create a collection of values to write
//		WriteValueCollection valuesToWrite = new(nodes.Select(_mapper.Map<WriteValue>));
//		WriteResponse result = new();

//		try
//		{
//			//Write the collection to the server
//			WriteResponse response = await client.Session.WriteAsync(null, valuesToWrite, cancellationToken);
//		}
//		catch (Exception e)
//		{
//			//handle Exception here
//			throw new OpcWriteException(e.Message, e.InnerException);
//		}
//		StatusCode? errorWritingCode = result.Results.FirstOrDefault(s => s != 0);
//		if (errorWritingCode is not null)
//		{
//			throw new OpcWriteException(errorWritingCode.ToString());
//		}
//		return result.Results.Count;

//	}


//	/// <summary>Writes data to a struct or UDT by node Id</summary>
//	/// <param name="nodeIdString">The node Id as strings</param>
//	/// <param name="dataToWrite">Structed node</param>
//	/// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
//	public void WriteStructUdt(IOpcClient client, string nodeIdString, OpcNode dataToWrite)
//	{
//		//Create a NodeId from the NodeIdString
//		NodeId nodeId = new(nodeIdString);

//		//Creat a WriteValueColelction
//		WriteValueCollection valuesToWrite = new();

//		//Create a WriteValue
//		WriteValue writevalue = new();

//		//Create a StatusCodeCollection
//		StatusCodeCollection results = new();

//		//Create a DiagnosticInfoCollection
//		DiagnosticInfoCollection diag = new();

//		//Create an ExtensionObject from the Structure given to this function
//		ExtensionObject writeExtObj = new();

//		DataValue dataValue = null;

//		//This is for regular Struct/UDT
//		if (dataToWrite[0][2].Contains("OF STRUCT/UDT"))
//		{
//			//Get array dimension
//			int startDim = dataToWrite[0][2].IndexOf("[") + 1;
//			int endDim = dataToWrite[0][2].IndexOf("]");

//			string arrDimString = dataToWrite[0][2][startDim..endDim];
//			int arrDim = Convert.ToInt32(arrDimString);

//			//Declar array of extension objects
//			ExtensionObject[] exObjArr = new ExtensionObject[arrDim];

//			//Split string list containing data to write
//			int index = 0;
//			for (int i = 0; i < arrDim; i++)
//			{
//				//Create temporary string list
//				List<string[]> splitData = new();

//				//Search for index pos
//				for (int j = index; j < dataToWrite.Count; j++)
//				{
//					if (!dataToWrite[j][0].Contains("[" + (i + 2).ToString() + "]"))
//					{
//						splitData.Add(dataToWrite[j]);
//					}
//					else
//					{
//						index = j + 1;
//						break;
//					}
//				}

//				//Determine lentgh of byte array needed to contain all data
//				long length = OpcWriterHelper.GetLengthOfDataToWrite(splitData);

//				//Create a byte array
//				//Parse dataToWrite to the byte array
//				byte[] bytesToWrite = ParseDataToByteArray(splitData, length);

//				//Copy data to extension object body
//				exObjArr[i] = new ExtensionObject
//				{
//					Body = bytesToWrite
//				};
//			}

//			//Turn the created ExtensionObject into a DataValue
//			dataValue = new DataValue(exObjArr);
//		}
//		//This is for array of Struct/UDT
//		else
//		{
//			//Determine lentgh of byte array needed to contain all data
//			long length = OpcWriterHelper.GetLengthOfDataToWrite(dataToWrite);

//			//Create a byte array
//			//Parse dataToWrite to the byte array
//			byte[] bytesToWrite = ParseDataToByteArray(dataToWrite, length);

//			//Copy data to extension object body
//			writeExtObj.Body = bytesToWrite;

//			//Turn the created ExtensionObject into a DataValue
//			dataValue = new DataValue(writeExtObj);
//		}

//		//Setup for the WriteValue
//		writevalue.NodeId = nodeId;
//		writevalue.Value = dataValue;
//		writevalue.AttributeId = Attributes.Value;
//		//Add the created value to the collection
//		valuesToWrite.Add(writevalue);

//		try
//		{
//			client.Session.Write(null, valuesToWrite, out results, out diag);
//		}
//		catch (Exception e)
//		{
//			//Handle Exception here
//			throw e;
//		}

//		//Check result codes
//		foreach (StatusCode result in results)
//		{
//			if (result.ToString() != "Good")
//			{
//				Exception e = new(result.ToString());
//				throw e;
//			}
//		}
//	}

//	#region Register/Unregister nodes Ids

//	/// <summary>Registers Node Ids to the server</summary>
//	/// <param name="nodeIdStrings">The node Ids as strings</param>
//	/// <returns>The registered Node Ids as strings</returns>
//	/// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
//	public IList<string> RegisterNodeIds(IList<string> nodeIdStrings)
//	{
//		NodeIdCollection nodesToRegister = new();
//		NodeIdCollection registeredNodes = new();
//		List<string> registeredNodeIdStrings = new();
//		foreach (string str in nodeIdStrings)
//		{
//			//Create a nodeId using the identifier string and add to list
//			nodesToRegister.Add(new NodeId(str));
//		}
//		try
//		{
//			//Register nodes
//			client.Session.RegisterNodes(null, nodesToRegister, out registeredNodes);

//			foreach (NodeId nodeId in registeredNodes)
//			{
//				registeredNodeIdStrings.Add(nodeId.ToString());
//			}

//			return registeredNodeIdStrings;
//		}
//		catch (Exception e)
//		{
//			//handle Exception here
//			throw e;
//		}
//	}

//	/// <summary>Unregister Node Ids to the server</summary>
//	/// <param name="nodeIdStrings">The node Ids as string</param>
//	/// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
//	public void UnregisterNodeIds(List<string> nodeIdStrings)
//	{
//		NodeIdCollection nodesToUnregister = new();
//		List<string> registeredNodeIdStrings = new();
//		foreach (string str in nodeIdStrings)
//		{
//			//Create a nodeId using the identifier string and add to list
//			nodesToUnregister.Add(new NodeId(str));
//		}
//		try
//		{
//			//Register nodes
//			client.Session.UnregisterNodes(null, nodesToUnregister);
//		}
//		catch (Exception e)
//		{
//			//handle Exception here
//			throw e;
//		}
//	}

//	#endregion Register/Unregister nodes Ids


//	/// <summary>Calls a method</summary>
//	/// <param name="methodIdString">The node Id as strings</param>
//	/// <param name="objectIdString">The object Id as strings</param>
//	/// <param name="inputData">The input argument data</param>
//	/// <returns>The list of output arguments</returns>
//	/// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
//	public IList<object> CallMethod(string methodIdString, string objectIdString, List<string[]> inputData)
//	{
//		//For calling a method we need it's node id and it's parent object's node id
//		NodeId methodNodeId = new(methodIdString);
//		NodeId objectNodeId = new(objectIdString);

//		//Declare an array of objects for the method's input arguments
//		object[] inputArguments = new object[inputData.Count];

//		//Parse data types first
//		//TBD: arrays for all types

//		bool parseCheck = false;
//		for (int i = 0; i < inputData.Count; i++)
//		{
//			if (inputData[i][1] == "SByte")
//			{
//				parseCheck = sbyte.TryParse(inputData[i][0], out sbyte value);
//				inputArguments[i] = value;
//			}
//			else if (inputData[i][1] == OpcConstants.TypeByte)
//			{
//				parseCheck = byte.TryParse(inputData[i][0], out byte value);
//				inputArguments[i] = value;
//			}
//			else if (inputData[i][1] == OpcConstants.TypeInt16)
//			{
//				parseCheck = short.TryParse(inputData[i][0], out short value);
//				inputArguments[i] = value;
//			}
//			else if (inputData[i][1].Contains("Int16["))
//			{
//				int pFrom = inputData[i][1].IndexOf("[") + 1;
//				int pTo = inputData[i][1].LastIndexOf("]");
//				string tempString = inputData[i][1][pFrom..pTo];
//				short[] value = new short[short.Parse(tempString)];

//				string[] tempArr = inputData[i][0].Split(';');

//				for (int j = 0; j < tempArr.Length; j++)
//				{
//					parseCheck = short.TryParse(tempArr[j], out value[j]);
//				}

//				inputArguments[i] = value;
//			}
//			else if (inputData[i][1] == OpcConstants.TypeInt32)
//			{
//				parseCheck = int.TryParse(inputData[i][0], out int value);
//				inputArguments[i] = value;
//			}
//			else if (inputData[i][1] == OpcConstants.TypeInt64)
//			{
//				parseCheck = long.TryParse(inputData[i][0], out long value);
//				inputArguments[i] = value;
//			}
//			else if (inputData[i][1] == OpcConstants.TypeBoolean)
//			{
//				parseCheck = bool.TryParse(inputData[i][0], out bool value);
//				inputArguments[i] = value;
//			}
//			else if (inputData[i][1] == OpcConstants.TypeString)
//			{
//				parseCheck = true;
//				inputArguments[i] = inputData[i][0];
//			}
//			else if (inputData[i][1] == OpcConstants.TypeFloat)
//			{
//				parseCheck = float.TryParse(inputData[i][0], out float value);
//				inputArguments[i] = value;
//			}
//			else if (inputData[i][1] == "DateTime")
//			{
//				DateTime value = new();
//				parseCheck = DateTime.TryParse(inputData[i][0], out value);
//				inputArguments[i] = value;
//			}
//			else if (inputData[i][1] == OpcConstants.TypeDouble)
//			{
//				parseCheck = double.TryParse(inputData[i][0], out double value);
//				inputArguments[i] = value;
//			}
//			else if (inputData[i][1] == OpcConstants.TypeUInt16)
//			{
//				parseCheck = ushort.TryParse(inputData[i][0], out ushort value);
//				inputArguments[i] = value;
//			}
//			else if (inputData[i][1] == OpcConstants.TypeUInt32)
//			{
//				parseCheck = uint.TryParse(inputData[i][0], out uint value);
//				inputArguments[i] = value;
//			}
//			else if (inputData[i][1] == OpcConstants.TypeUInt64)
//			{
//				parseCheck = ulong.TryParse(inputData[i][0], out ulong value);
//				inputArguments[i] = value;
//			}
//			else if (inputData[i][1] == OpcConstants.TypeDouble)
//			{
//				parseCheck = double.TryParse(inputData[i][0], out double value);
//				inputArguments[i] = value;
//			}
//			else if (inputData[i][1] == "ByteString")
//			{
//				int NumberChars = inputData[i][0].Length;
//				if (NumberChars % 2 == 1)
//				{
//					Exception e = new("Check length of ByteString");
//					throw e;
//				}
//				byte[] value = new byte[NumberChars / 2];
//				for (int j = 0; j < NumberChars; j += 2)
//				{
//					value[j / 2] = Convert.ToByte(inputData[i][0].Substring(j, 2), 16);
//				}
//				parseCheck = true;
//				inputArguments[i] = value;
//			}
//			else
//			{
//				Exception e = new("Complex data type cannot be parsed.");
//				throw e;
//			}

//			if (!parseCheck)
//			{
//				Exception e = new("Please validate input value");
//				throw e;
//			}
//		}

//		//Declare a list of objects for the method's output arguments
//		IList<object> outputArguments = new List<object>();

//		//Call the method
//		return client.Session.Call(objectNodeId, methodNodeId, inputArguments);
//	}


//	/// <summary>Parses data to write to a byte array</summary>
//	/// <param name="dataToWrite">The data to write as string[3]; string[0] = tag name, string[1] = value, string[2] = opc data type</param>
//	/// <param name="dataLength">The length of the data to write</param>
//	/// <returns>The parsed byte array</returns>
//	/// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
//	private static byte[] ParseDataToByteArray(IList<string[]> dataToWrite, long dataLength)
//	{
//		byte[] bytesToWrite = new byte[dataLength];
//		int convertIndex = 0;
//		int arraySize = 0;
//		foreach (string[] val in dataToWrite)
//		{
//			if (val[2] == OpcConstants.TypeBoolean && arraySize <= 0)
//			{
//				bool tempBool = Convert.ToBoolean(val[1]);
//				bytesToWrite[convertIndex] = Convert.ToByte(tempBool);
//				convertIndex++;
//			}
//			else if (val[2] == OpcConstants.TypeInt16 && arraySize <= 0)
//			{
//				Array.Copy(BitConverter.GetBytes(Convert.ToInt16(val[1])), 0, bytesToWrite, convertIndex, 2);
//				convertIndex += 2;
//			}
//			else if (val[2] == OpcConstants.TypeUInt16 && arraySize <= 0)
//			{
//				Array.Copy(BitConverter.GetBytes(Convert.ToUInt16(val[1])), 0, bytesToWrite, convertIndex, 2);
//				convertIndex += 2;
//			}
//			else if (val[2] == OpcConstants.TypeFloat && arraySize <= 0)
//			{
//				Array.Copy(BitConverter.GetBytes(Convert.ToSingle(val[1])), 0, bytesToWrite, convertIndex, 4);
//				convertIndex += 4;
//			}
//			else if (val[2] == OpcConstants.TypeDouble && arraySize <= 0)
//			{
//				Array.Copy(BitConverter.GetBytes(Convert.ToDouble(val[1])), 0, bytesToWrite, convertIndex, 8);
//				convertIndex += 8;
//			}
//			else if (val[2] == OpcConstants.TypeInt32 && !val[0].Contains("_Size") && arraySize <= 0 || val[2] == OpcConstants.TypeUInt32 && arraySize <= 0)
//			{
//				Array.Copy(BitConverter.GetBytes(Convert.ToUInt32(val[1])), 0, bytesToWrite, convertIndex, 4);
//				convertIndex += 4;
//			}
//			else if (val[2] == OpcConstants.TypeInt64 && arraySize <= 0)
//			{
//				Array.Copy(BitConverter.GetBytes(Convert.ToInt64(val[1])), 0, bytesToWrite, convertIndex, 8);
//				convertIndex += 8;
//			}
//			else if (val[2] == OpcConstants.TypeUInt64 && arraySize <= 0)
//			{
//				Array.Copy(BitConverter.GetBytes(Convert.ToUInt64(val[1])), 0, bytesToWrite, convertIndex, 8);
//				convertIndex += 8;
//			}
//			else if (val[2] == OpcConstants.TypeByte && arraySize <= 0)
//			{
//				bytesToWrite[convertIndex] = Convert.ToByte(val[1]);
//				convertIndex++;
//			}
//			else if (val[2] == OpcConstants.TypeString && arraySize <= 0 || val[2] == OpcConstants.TypeCharArray)
//			{
//				Array.Copy(BitConverter.GetBytes(val[1].Length), 0, bytesToWrite, convertIndex, 4);
//				convertIndex += 4;
//				foreach (char c in val[1])
//				{
//					bytesToWrite[convertIndex] = Convert.ToByte(c);
//					convertIndex++;
//				}
//			}
//			else if (val[2] == OpcConstants.TypeInt32 && val[0].Contains("_Size"))
//			{
//				Array.Copy(BitConverter.GetBytes(Convert.ToUInt32(val[1])), 0, bytesToWrite, convertIndex, 4);
//				arraySize = Convert.ToInt32(val[1]);
//				convertIndex += 4;
//			}
//			else if (val[2] == OpcConstants.TypeByte && arraySize > 0)
//			{
//				string tempString = "";
//				foreach (char c in val[1])
//				{
//					if (c != ';')
//					{
//						tempString = string.Concat(tempString, c);
//					}
//					else
//					{
//						bytesToWrite[convertIndex] = Convert.ToByte(tempString);
//						convertIndex++;
//						tempString = "";
//					}
//				}
//				arraySize = 0;
//			}
//			else if (val[2] == OpcConstants.TypeInt16 && arraySize > 0)
//			{
//				string tempString = "";
//				foreach (char c in val[1])
//				{
//					if (c != ';')
//					{
//						tempString = string.Concat(tempString, c);
//					}
//					else
//					{
//						Array.Copy(BitConverter.GetBytes(Convert.ToInt16(tempString)), 0, bytesToWrite, convertIndex, 2);
//						convertIndex += 2;
//						tempString = "";
//					}
//				}
//				arraySize = 0;
//			}
//			else if (val[2] == OpcConstants.TypeUInt16 && arraySize > 0)
//			{
//				string tempString = "";
//				foreach (char c in val[1])
//				{
//					if (c != ';')
//					{
//						tempString = string.Concat(tempString, c);
//					}
//					else
//					{
//						Array.Copy(BitConverter.GetBytes(Convert.ToUInt16(tempString)), 0, bytesToWrite, convertIndex, 2);
//						convertIndex += 2;
//						tempString = "";
//					}
//				}
//				arraySize = 0;
//			}
//			else if (val[2] == OpcConstants.TypeInt32 && arraySize > 0)
//			{
//				string tempString = "";
//				foreach (char c in val[1])
//				{
//					if (c != ';')
//					{
//						tempString = string.Concat(tempString, c);
//					}
//					else
//					{
//						Array.Copy(BitConverter.GetBytes(Convert.ToInt32(tempString)), 0, bytesToWrite, convertIndex, 4);
//						convertIndex += 4;
//						tempString = "";
//					}
//				}
//				arraySize = 0;
//			}
//			else if (val[2] == OpcConstants.TypeUInt32 && arraySize > 0)
//			{
//				string tempString = "";
//				foreach (char c in val[1])
//				{
//					if (c != ';')
//					{
//						tempString = string.Concat(tempString, c);
//					}
//					else
//					{
//						Array.Copy(BitConverter.GetBytes(Convert.ToUInt32(tempString)), 0, bytesToWrite, convertIndex, 4);
//						convertIndex += 4;
//						tempString = "";
//					}
//				}
//				arraySize = 0;
//			}
//			else if (val[2] == OpcConstants.TypeInt64 && arraySize > 0)
//			{
//				string tempString = "";
//				foreach (char c in val[1])
//				{
//					if (c != ';')
//					{
//						tempString = string.Concat(tempString, c);
//					}
//					else
//					{
//						Array.Copy(BitConverter.GetBytes(Convert.ToInt64(tempString)), 0, bytesToWrite, convertIndex, 8);
//						convertIndex += 8;
//						tempString = "";
//					}
//				}
//				arraySize = 0;
//			}
//			else if (val[2] == OpcConstants.TypeUInt64 && arraySize > 0)
//			{
//				string tempString = "";
//				foreach (char c in val[1])
//				{
//					if (c != ';')
//					{
//						tempString = string.Concat(tempString, c);
//					}
//					else
//					{
//						Array.Copy(BitConverter.GetBytes(Convert.ToUInt64(tempString)), 0, bytesToWrite, convertIndex, 8);
//						convertIndex += 8;
//						tempString = "";
//					}
//				}
//				arraySize = 0;
//			}
//			else if (val[2] == OpcConstants.TypeFloat && arraySize > 0)
//			{
//				string tempString = "";
//				foreach (char c in val[1])
//				{
//					if (c != ';')
//					{
//						tempString = string.Concat(tempString, c);
//					}
//					else
//					{
//						Array.Copy(BitConverter.GetBytes(Convert.ToSingle(tempString)), 0, bytesToWrite, convertIndex, 4);
//						convertIndex += 4;
//						tempString = "";
//					}
//				}
//				arraySize = 0;
//			}
//			else if (val[2] == OpcConstants.TypeString && arraySize > 0)
//			{
//				string[] tempStringArr = new string[arraySize];
//				tempStringArr = val[1].Split(';');
//				for (int ii = 0; ii < arraySize; ii++)
//				{
//					Array.Copy(BitConverter.GetBytes(tempStringArr[ii].Length), 0, bytesToWrite, convertIndex, 4);
//					convertIndex += 4;
//					foreach (char c in tempStringArr[ii])
//					{
//						bytesToWrite[convertIndex] = Convert.ToByte(c);
//						convertIndex++;
//					}
//				}
//				arraySize = 0;
//			}
//			else if (!val[2].Contains(OpcConstants.TypeStruct))
//			{
//				Exception e = new("Can't covert " + val[0] + ".");
//				throw e;
//			}
//		}
//		return bytesToWrite;
//	}

//	/// <summary>Parses a byte array to objects containing tag names and tag data types</summary>
//	/// <param name="varList">List of object containing tag names and tag data types</param>
//	/// <param name="byteResult">A byte array to parse</param>
//	/// <returns>A list of string[3]; string[0] = tag name, string[1] = value, string[2] = opc data type</returns>
//	/// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
//	private static List<string[]> ParseDataToTagsFromDictionary(List<object> varList, byte[] byteResult)
//	{
//		//Define result list to return var name, var value and var data type
//		List<string[]> resultStringList = new();

//		//Byte decoding index
//		int index = 0;

//		//Int used to decode arrays
//		int arraylength = 0;

//		//Start decoding for opc data types
//		foreach (object val in varList)
//		{
//			string[] dataReferenceStringArray = new string[3];
//			dataReferenceStringArray[0] = ((string[])val)[0];

//			if (((string[])val)[1] == OpcConstants.TypeBoolean && arraylength <= 0)
//			{
//				dataReferenceStringArray[1] = BitConverter.ToBoolean(byteResult, index).ToString();
//				index++;
//			}
//			else if (((string[])val)[1] == OpcConstants.TypeInt16 && arraylength <= 0)
//			{
//				dataReferenceStringArray[1] = BitConverter.ToInt16(byteResult, index).ToString();
//				index += 2;
//			}
//			else if (((string[])val)[1] == OpcConstants.TypeInt16 && arraylength > 0)
//			{
//				short[] tempArray = new short[arraylength];
//				for (int i = 0; i < arraylength; i++)
//				{
//					tempArray[i] = BitConverter.ToInt16(byteResult, index);
//					index += 2;
//				}
//				dataReferenceStringArray[1] = string.Join(";", tempArray);
//				dataReferenceStringArray[1] = string.Concat(dataReferenceStringArray[1], ";");
//				arraylength = 0;
//			}
//			else if (((string[])val)[1] == OpcConstants.TypeUInt16 && arraylength > 0)
//			{
//				ushort[] tempArray = new ushort[arraylength];
//				for (int i = 0; i < arraylength; i++)
//				{
//					tempArray[i] = BitConverter.ToUInt16(byteResult, index);
//					index += 2;
//				}
//				dataReferenceStringArray[1] = string.Join(";", tempArray);
//				dataReferenceStringArray[1] = string.Concat(dataReferenceStringArray[1], ";");
//				arraylength = 0;
//			}
//			else if (((string[])val)[1] == OpcConstants.TypeInt32 && arraylength <= 0 && !((string[])val)[0].Contains("_Size"))
//			{
//				dataReferenceStringArray[1] = BitConverter.ToInt32(byteResult, index).ToString();
//				index += 4;
//			}
//			else if (((string[])val)[1] == OpcConstants.TypeInt32 && arraylength > 0)
//			{
//				int[] tempArray = new int[arraylength];
//				for (int i = 0; i < arraylength; i++)
//				{
//					tempArray[i] = BitConverter.ToInt32(byteResult, index);
//					index += 4;
//				}
//				dataReferenceStringArray[1] = string.Join(";", tempArray);
//				dataReferenceStringArray[1] = string.Concat(dataReferenceStringArray[1], ";");
//				arraylength = 0;
//			}
//			else if (((string[])val)[1] == OpcConstants.TypeUInt32 && arraylength > 0)
//			{
//				uint[] tempArray = new uint[arraylength];
//				for (int i = 0; i < arraylength; i++)
//				{
//					tempArray[i] = BitConverter.ToUInt32(byteResult, index);
//					index += 4;
//				}
//				dataReferenceStringArray[1] = string.Join(";", tempArray);
//				dataReferenceStringArray[1] = string.Concat(dataReferenceStringArray[1], ";");
//				arraylength = 0;
//			}
//			else if (((string[])val)[1] == OpcConstants.TypeInt64 && arraylength <= 0)
//			{
//				dataReferenceStringArray[1] = BitConverter.ToInt64(byteResult, index).ToString();
//				index += 8;
//			}
//			else if (((string[])val)[1] == OpcConstants.TypeUInt64 && arraylength > 0)
//			{
//				ulong[] tempArray = new ulong[arraylength];
//				for (int i = 0; i < arraylength; i++)
//				{
//					tempArray[i] = BitConverter.ToUInt64(byteResult, index);
//					index += 8;
//				}
//				dataReferenceStringArray[1] = string.Join(";", tempArray);
//				dataReferenceStringArray[1] = string.Concat(dataReferenceStringArray[1], ";");
//				arraylength = 0;
//			}
//			else if (((string[])val)[1] == OpcConstants.TypeFloat && arraylength <= 0)
//			{
//				dataReferenceStringArray[1] = BitConverter.ToSingle(byteResult, index).ToString();
//				index += 4;
//			}
//			else if (((string[])val)[1] == OpcConstants.TypeFloat && arraylength > 0)
//			{
//				float[] tempArray = new float[arraylength];
//				for (int i = 0; i < arraylength; i++)
//				{
//					tempArray[i] = BitConverter.ToSingle(byteResult, index);
//					index += 4;
//				}
//				dataReferenceStringArray[1] = string.Join(";", tempArray);
//				dataReferenceStringArray[1] = string.Concat(dataReferenceStringArray[1], ";");
//				arraylength = 0;
//			}
//			else if (((string[])val)[1] == OpcConstants.TypeDouble && arraylength <= 0)
//			{
//				dataReferenceStringArray[1] = BitConverter.ToDouble(byteResult, index).ToString();
//				index += 8;
//			}
//			else if (((string[])val)[1] == OpcConstants.TypeDouble && arraylength > 0)
//			{
//				double[] tempArray = new double[arraylength];
//				for (int i = 0; i < arraylength; i++)
//				{
//					tempArray[i] = BitConverter.ToDouble(byteResult, index);
//					index += 8;
//				}
//				dataReferenceStringArray[1] = string.Join(";", tempArray);
//				dataReferenceStringArray[1] = string.Concat(dataReferenceStringArray[1], ";");
//				arraylength = 0;
//			}
//			else if (((string[])val)[1] == OpcConstants.TypeString && arraylength <= 0)
//			{
//				int stringlength = BitConverter.ToInt32(byteResult, index);
//				index += 4;
//				if (stringlength > 0)
//				{
//					dataReferenceStringArray[1] = Encoding.UTF8.GetString(byteResult, index, stringlength);
//					index += stringlength;
//				}
//				else
//				{
//					dataReferenceStringArray[1] = "";
//				}
//			}
//			else if (((string[])val)[1] == OpcConstants.TypeCharArray)
//			{
//				int stringlength = BitConverter.ToInt32(byteResult, index);
//				index += 4;
//				if (stringlength > 0)
//				{
//					dataReferenceStringArray[1] = Encoding.UTF8.GetString(byteResult, index, stringlength);
//					index += stringlength;
//				}
//				else
//				{
//					dataReferenceStringArray[1] = "";
//				}
//			}
//			else if (((string[])val)[1] == OpcConstants.TypeUInt16 && arraylength <= 0)
//			{
//				dataReferenceStringArray[1] = BitConverter.ToUInt16(byteResult, index).ToString();
//				index += 2;
//			}
//			else if (((string[])val)[1] == OpcConstants.TypeUInt32 && arraylength <= 0)
//			{
//				dataReferenceStringArray[1] = BitConverter.ToUInt32(byteResult, index).ToString();
//				index += 4;
//			}
//			else if (((string[])val)[1] == OpcConstants.TypeInt64 && arraylength > 0)
//			{
//				long[] tempArray = new long[arraylength];
//				for (int i = 0; i < arraylength; i++)
//				{
//					tempArray[i] = BitConverter.ToInt64(byteResult, index);
//					index += 8;
//				}
//				dataReferenceStringArray[1] = string.Join(";", tempArray);
//				dataReferenceStringArray[1] = string.Concat(dataReferenceStringArray[1], ";");
//				arraylength = 0;
//			}
//			else if (((string[])val)[1] == OpcConstants.TypeUInt64 && arraylength <= 0)
//			{
//				dataReferenceStringArray[1] = BitConverter.ToUInt64(byteResult, index).ToString();
//				index += 8;
//			}
//			else if (((string[])val)[1] == OpcConstants.TypeByte && arraylength <= 0)
//			{
//				dataReferenceStringArray[1] = byteResult[index].ToString();
//				index++;
//			}
//			else if (((string[])val)[1] == OpcConstants.TypeInt32 && ((string[])val)[0].Contains("_Size"))
//			{
//				arraylength = BitConverter.ToInt32(byteResult, index);
//				dataReferenceStringArray[1] = arraylength.ToString();
//				index += 4;
//			}
//			else if (((string[])val)[1] == OpcConstants.TypeByte && arraylength > 0)
//			{
//				int[] tempArray = new int[arraylength];
//				for (int i = 0; i < arraylength; i++)
//				{
//					tempArray[i] = byteResult[index];
//					index++;
//				}
//				dataReferenceStringArray[1] = string.Join(";", tempArray);
//				dataReferenceStringArray[1] = string.Concat(dataReferenceStringArray[1], ";");
//				arraylength = 0;
//			}
//			else if (((string[])val)[1] == OpcConstants.TypeString && arraylength > 0)
//			{
//				for (int i = 0; i < arraylength; i++)
//				{
//					int stringlength = BitConverter.ToInt32(byteResult, index);
//					index += 4;
//					if (stringlength > 0)
//					{
//						dataReferenceStringArray[1] = string.Concat(dataReferenceStringArray[1], Encoding.UTF8.GetString(byteResult, index, stringlength));
//						dataReferenceStringArray[1] = string.Concat(dataReferenceStringArray[1], ";");
//						index += stringlength;
//					}
//					else
//					{
//						dataReferenceStringArray[1] += ";";
//					}
//				}
//				arraylength = 0;
//			}
//			else if (((string[])val)[1].Contains(OpcConstants.TypeStruct) && arraylength <= 0)
//			{
//				dataReferenceStringArray[1] = "..";
//			}
//			else
//			{
//				Exception e = new("Data type is too complex to be parsed." + Environment.NewLine + "Please avoid array of UDT/Struct inside UDT/Struct");
//				throw e;
//			}
//			dataReferenceStringArray[2] = ((string[])val)[1];
//			resultStringList.Add(dataReferenceStringArray);
//		}
//		return resultStringList;
//	}

//	/// <summary>Parses a XML string for the a default data type</summary>
//	/// <param name="xmlStringToParse">The XML string containing data type information</param>
//	/// <param name="stringToParserFor">The data type as string to search for</param>
//	/// <returns>The created objects after parsing for default data type</returns>
//	/// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
//	private static List<object> ParseTypeDictionary(string xmlStringToParse, string stringToParserFor)
//	{
//		List<object> varList = new();

//		//Remove last XML sign and create a XML document out of the dictionary string
//		if (xmlStringToParse.EndsWith("\n"))
//		{
//			xmlStringToParse = xmlStringToParse.Remove(xmlStringToParse.Length - 1);
//		}
//		XmlDocument docToParse = new();
//		docToParse.LoadXml(xmlStringToParse);

//		//Get a XML node list of objectes named by "stringToParseFor"
//		docToParse.GetElementsByTagName(stringToParserFor);
//		XmlNodeList nodeList = docToParse.GetElementsByTagName("opc:StructuredType");

//		XmlNode foundNode = null;

//		//search for the attribute name == "stringToParseFor"
//		foreach (XmlNode node in nodeList)
//		{
//			if (node.Attributes["Name"].Value == stringToParserFor)
//			{
//				foundNode = node;
//				break;
//			}
//		}

//		//check if attribute name was found
//		if (foundNode == null)
//		{
//			return null;
//		}

//		//get child nodes of parent node with attribute name == "stringToParseFor" and parse for var name and var type
//		foreach (XmlNode node in foundNode.ChildNodes)
//		{
//			string[] dataReferenceStringArray = new string[2];

//			dataReferenceStringArray[0] = node.Attributes["Name"].Value;
//			dataReferenceStringArray[1] = node.Attributes["TypeName"].Value;
//			if (!dataReferenceStringArray[1].Contains("tns:"))
//			{
//				dataReferenceStringArray[1] = dataReferenceStringArray[1].Remove(0, 4);
//			}
//			else
//			{
//				dataReferenceStringArray[1] = dataReferenceStringArray[1].Remove(0, 4);
//				dataReferenceStringArray[1] = dataReferenceStringArray[1].Insert(0, "STRUCT/UDT:");
//			}

//			varList.Add(dataReferenceStringArray);
//		}

//		//Check if result contains another struct/UDT inside and parse for var name and var type
//		//Note: This check is consistent even if there are more structs/UDTs inside of structs/UDTs
//		for (int count = 0; count < varList.Count; count++)
//		{
//			object varObject = varList[count];
//			//"tns:" indicates another strucht/UDT
//			if (((string[])varObject)[1].Contains("STRUCT/UDT:"))
//			{
//				XmlNode innerNode = null;
//				foreach (XmlNode anotherNode in nodeList)
//				{
//					if (anotherNode.Attributes["Name"].Value == ((string[])varObject)[1].Remove(0, 11))
//					{
//						innerNode = anotherNode;
//						break;
//					}
//				}

//				if (innerNode == null)
//				{
//					return null;
//				}

//				int i = 0;
//				foreach (XmlNode innerChildNode in innerNode.ChildNodes)
//				{
//					string[] innerDataReferenceStringArray = new string[2];
//					innerDataReferenceStringArray[0] = innerChildNode.Attributes["Name"].Value;
//					innerDataReferenceStringArray[1] = innerChildNode.Attributes["TypeName"].Value;
//					if (!innerDataReferenceStringArray[1].Contains("tns:"))
//					{
//						innerDataReferenceStringArray[1] = innerDataReferenceStringArray[1].Remove(0, 4);
//					}
//					else
//					{
//						innerDataReferenceStringArray[1] = innerDataReferenceStringArray[1].Remove(0, 4);
//						innerDataReferenceStringArray[1] = innerDataReferenceStringArray[1].Insert(0, "STRUCT/UDT:");
//					}

//					varList.Insert(varList.IndexOf(varObject) + 1 + i, innerDataReferenceStringArray);
//					i++;

//					if (i == innerNode.ChildNodes.Count)
//					{
//						string[] innerDataReferenceStringArrayEnd = new string[2];
//						innerDataReferenceStringArrayEnd[0] = ((string[])varObject)[0];
//						innerDataReferenceStringArrayEnd[1] = "END_STRUCT/UDT";
//						varList.Insert(varList.IndexOf(varObject) + 1 + i, innerDataReferenceStringArrayEnd);
//					}
//				}
//			}
//		}
//		return varList;
//	}
//}
