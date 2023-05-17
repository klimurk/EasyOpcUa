using Opc.Ua;
using Opc.Ua.Client;
using OpcUa.Domain.Contracts.Client;
using System.Text;
using OpcUa.Persistance.Helpers;
using OpcUa.Persistance.Exceptions.Reader;
using OpcUa.Persistance.Exceptions.Related;
using OpcUa.Domain.Basics;
using Newtonsoft.Json.Schema;

namespace Woodnailer.Application.Opc.Client;

public class OpcReader
{


	#region Browse

	/// <summary>Browses a node ID provided by a ReferenceDescription</summary>
	/// <param name="refDesc">The ReferenceDescription</param>
	/// <returns>ReferenceDescriptionCollection of found nodes</returns>
	/// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
	public ReferenceDescriptionCollection BrowseReference(IOpcClient client, ReferenceDescription refDesc)
	{
		// todo: null change
		//Create a NodeId using the selected ReferenceDescription as browsing starting point
		NodeId nodeId = ExpandedNodeId.ToNodeId(refDesc.NodeId, null);
		try
		{
			//Browse from starting point for all object types
			var collection = ExtractReferenceCollection(client, nodeId);
			return collection;
		}
		catch (Exception)
		{
			//handle Exception here
			throw;
		}
	}
	public ReferenceDescriptionCollection BrowseNode(IOpcClient client, Node node)
	{
		//Create a NodeId using the selected ReferenceDescription as browsing starting point
		NodeId nodeId = ExpandedNodeId.ToNodeId(node.NodeId, null);
		try
		{
			//Browse from starting point for all object types
			var collection = ExtractReferenceCollection(client, nodeId);
			return collection;
		}
		catch (Exception)
		{
			//handle Exception here
			throw;
		}
	}

	private ReferenceDescriptionCollection ExtractReferenceCollection(IOpcClient client, NodeId nodeId)
	{
		client.Session.Browse(null, view: null, nodeId, 0u, BrowseDirection.Forward, ReferenceTypeIds.HierarchicalReferences, true, 0, out byte[] continuationPoint, out ReferenceDescriptionCollection referenceDescriptionCollection);
		// todo: check arra not empty
		while (continuationPoint != null)
		{
			client.Session.BrowseNext(null, false, continuationPoint, out byte[] revisedContinuationPoint, out ReferenceDescriptionCollection nextreferenceDescriptionCollection);
			referenceDescriptionCollection.AddRange(nextreferenceDescriptionCollection);
			continuationPoint = revisedContinuationPoint;
		}

		return referenceDescriptionCollection;
	}

	public bool CheckNodeIsNotVariable(IOpcClient client, Node node)
	{
		//Create a NodeId using the selected ReferenceDescription as browsing starting point
		NodeId nodeId = ExpandedNodeId.ToNodeId(node.NodeId, null);
		try
		{
			//Browse from starting point for all object types
			client.Session.Browse(null, null, nodeId, 0u, BrowseDirection.Forward, ReferenceTypeIds.HierarchicalReferences, true, 0, out byte[] continuationPoint, out ReferenceDescriptionCollection referenceDescriptionCollection);
			return referenceDescriptionCollection?.Count > 0;
		}
		catch (Exception)
		{
			//handle Exception here
			throw;
		}
	}


	/// <summary>Browses the root folder of an OPC UA server.</summary>
	/// <returns>ReferenceDescriptionCollection of found nodes</returns>
	/// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
	public ReferenceDescriptionCollection BrowseRoot(IOpcClient client)
	{
		try
		{
			//Browse the RootFolder for variables, objects and methods
			client.Session.Browse(null, null, ObjectIds.RootFolder, 0u, BrowseDirection.Forward, ReferenceTypeIds.HierarchicalReferences, true, (uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method, out byte[] continuationPoint, out ReferenceDescriptionCollection referenceDescriptionCollection);
			return referenceDescriptionCollection;
		}
		catch (Exception e)
		{
			// exception
			return null;
		}
	}

	#endregion Browse

	/// <summary>Reads a node by node Id</summary>
	/// <param name="nodeIdString">The node Id as string</param>
	/// <returns>The read node</returns>
	/// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
	public Node GetNode(IOpcClient client, string nodeIdString)
	{
		if (string.IsNullOrEmpty(nodeIdString.Trim())) return null;
		//Create a node
		try
		{
			//Read the dataValue
			var result = client.Session.ReadNode(new NodeId(nodeIdString));
			return result;
		}
		catch (Exception e)
		{
			return null;
			//handle Exception here
		}
	}
	/// <summary>Reads values from node Ids</summary>
	/// <param name="nodeIdStrings">The node Ids as strings</param>
	/// <returns>The read values as strings</returns>
	/// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
	#region Read Values

	#region Read List Values
	public IEnumerable<T> ReadNodeListValues<T>(IOpcClient client, IEnumerable<string> nodeIdStrings) where T : struct, IComparable, IConvertible, ISpanFormattable
	{
		List<NodeId> nodeIds = nodeIdStrings.Select(s => new NodeId(s)).ToList();

		return GetNodeListValues<T>(client, nodeIds);
	}
	public IEnumerable<T> ReadNodeListClassValues<T>(IOpcClient client, IEnumerable<string> nodeIdStrings) where T : class
	{
		List<NodeId> nodeIds = nodeIdStrings.Select(s => new NodeId(s)).ToList();

		return GetNodeListValues<T>(client, nodeIds);
	}
	public IEnumerable<T> ReadNodeListValues<T>(IOpcClient client, IEnumerable<Node> nodes) where T : struct, IComparable, IConvertible, ISpanFormattable
	{
		List<NodeId> nodeIds = nodes.Select(s => s.NodeId).ToList();

		return GetNodeListValues<T>(client, nodeIds);
	}
	public IEnumerable<T> ReadNodeListClassValues<T>(IOpcClient client, IEnumerable<Node> nodes) where T : class
	{
		List<NodeId> nodeIds = nodes.Select(s => s.NodeId).ToList();

		return GetNodeListValues<T>(client, nodeIds);
	}

	private static IEnumerable<T> GetNodeListValues<T>(IOpcClient client, IList<NodeId> nodeIds)
	{
		List<object> values = new();
		List<ServiceResult> serviceResults = new();
		List<Type> types = Enumerable.Repeat(typeof(T), nodeIds.Count).ToList();
		try
		{
			//Read the dataValues
			client.Session.ReadValues(nodeIds, types, out values, out serviceResults);
		}
		catch (Exception e)
		{
			//handle Exception here
			throw e;
		}
		foreach (var svResult in serviceResults.Where(svResult => svResult != ServiceResult.Good))
		{
			// todo: describe exception
			throw new ReadNodeException(svResult.ToString());
		}
		var result = values.Select(s => (T)s);
		// exclude bytes with symbol '-'
		if (typeof(byte).IsAssignableFrom(typeof(T)))
		{
			List<byte> byteArr = result.OfType<byte>().ToList();
			byte exclude = 45; // symbol '-'
			if (byteArr.Any()) byteArr.RemoveAll(s => s == exclude);

		}

		//List<object> byteArrs = result.Where(s => s is byte[]).ToList();
		//byteArrs.ForEach(s => s = ((byte[])s).Where(b => b != exclude).ToArray());

		return result;
	}
	#endregion

	#region Read List Values Async
	public async Task<IEnumerable<DataValue>> ReadNodeListValuesAsync(IOpcClient client, IEnumerable<string> nodeIdStrings, CancellationToken cancellationToken)
	{
		List<NodeId> nodeIds = nodeIdStrings.Select(s => new NodeId(s)).ToList();

		return await GetNodeListValuesAsync(client, nodeIds, cancellationToken);
	}
	public async Task<IEnumerable<DataValue>> ReadNodeListValuesAsync(IOpcClient client, IEnumerable<Node> nodes, CancellationToken cancellationToken)
	{
		List<NodeId> nodeIds = nodes.Select(s => s.NodeId).ToList();

		return await GetNodeListValuesAsync(client, nodeIds, cancellationToken);
	}
	private async Task<IEnumerable<DataValue>> GetNodeListValuesAsync(IOpcClient client, IList<NodeId> nodeIds, CancellationToken cancellationToken)
	{
		DataValueCollection result;
		IList<ServiceResult> serviceResults;

		try
		{
			//Read the dataValues
			(result, serviceResults) = await client.Session.ReadValuesAsync(nodeIds, cancellationToken);
		}
		catch (Exception e)
		{
			//handle Exception here
			throw e;
		}
		foreach (var svResult in serviceResults.Where(svResult => svResult.ToString() != "Good"))
		{
			throw new ReadNodeException(svResult.ToString());
		}

		// exclude bytes with symbol '-'
		byte exclude = 45; // symbol '-'
		return result.Where(s => s.Value is byte val && val != exclude);
	}
	#endregion

	#region Read Value
	public T ReadNodeValue<T>(IOpcClient client, string nodeIdString) where T : struct, IComparable, IConvertible, ISpanFormattable
	{
		NodeId nodeIds = new(nodeIdString);

		return GetNodeValue<T>(client, nodeIds);
	}
	public T ReadNodeValue<T>(IOpcClient client, Node node) where T : struct, IComparable, IConvertible, ISpanFormattable
	{
		return GetNodeValue<T>(client, node.NodeId);
	}
	public T ReadNodeClassValue<T>(IOpcClient client, string nodeIdString) where T : class
	{
		NodeId nodeIds = new(nodeIdString);

		return GetNodeValue<T>(client, nodeIds);
	}
	public T ReadNodeClassValue<T>(IOpcClient client, Node node) where T : class
	{
		return GetNodeValue<T>(client, node.NodeId);
	}
	private static T GetNodeValue<T>(IOpcClient client, NodeId nodeId)
	{
		object result;

		try
		{
			//Read the dataValues
			result = client.Session.ReadValue(nodeId, typeof(T));
		}
		catch (Exception e)
		{
			//handle Exception here
			throw e;
		}
		return (T)result;
	}
	#endregion

	#region Read Value Async
	public async Task<DataValue> ReadNodeValueAsync(IOpcClient client, string nodeIdString, CancellationToken cancellationToken)
	{
		NodeId nodeIds = new(nodeIdString);

		return await GetNodeValueAsync(client, nodeIds, cancellationToken);
	}
	public async Task<DataValue> ReadNodeValueAsync(IOpcClient client, Node node, CancellationToken cancellationToken)
	{
		return await GetNodeValueAsync(client, node.NodeId, cancellationToken);
	}
	private async Task<DataValue> GetNodeValueAsync(IOpcClient client, NodeId nodeIds, CancellationToken cancellationToken)
	{
		DataValue result;
		try
		{
			result = await client.Session.ReadValueAsync(nodeIds, cancellationToken);
		}
		catch (Exception e)
		{
			throw e;
		}
		return result;
	}
	#endregion

	#endregion

	/// <summary>Reads a struct or UDT by node Id</summary>
	/// <param name="nodeIdString">The node Id as strings</param>
	/// <returns>The read struct/UDT elements as a list of string[3]; string[0] = tag name, string[1] = value, string[2] = opc data type</returns>
	/// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
	public OpcNode ReadStructUdt(IOpcClient client, string nodeIdString)
	{
		//Read the struct
		IList<NodeId> nodeIds = new List<NodeId>() { new(nodeIdString) };
		List<ServiceResult> serviceResults = new();
		List<object> values = new();
		IList<Type> types = new List<Type>() { null };
		try
		{
			client.Session.ReadValues(nodeIds, types, out values, out serviceResults);
		}
		catch (Exception e)
		{
			//Handle Exception here
			throw e;
		}
		//Check result codes
		ServiceResult? errorResult = serviceResults.FirstOrDefault(s => s.ToString() != "Good");
		if (errorResult is not null)
		{
			throw new ReadNodeException(errorResult.ToString());
		}



		//Define result list to return var name and var value
		var result = new OpcNode();

		//Get the type dictionary of desired struct/UDT and the name of desired var to parse
		string parseString;
		string xmlString = GetTypeDictionary(nodeIdString, client, out parseString);
		//Parse xmlString to create objects of the struct/UDT containing var name and var data type
		IEnumerable<OpcNode> varList = OpcXmlParser.ParseOpcStructs(xmlString, parseString);
		//Create an empty byte-array to store ExtensionObject.Body (containing the whole binary data of the desired strucht/UDT) into
		byte[] readBinaryData;
		foreach (object val in values)
		{
			//Cast object to ExtensionObject
			//If encodable == null there might be an array
			if (val is ExtensionObject encodeable)
			{
				readBinaryData = (byte[])encodeable.Body;
				//Write the body(=data) of the ExtensionObject into the byte-array
				IEnumerable<OpcNode> Tags = OpcXmlParser.InsertValuesInStruct(varList, readBinaryData);
				//Check for data types and parse byte array
				result.InnerNodes.AddRange(Tags);
			}
			else
			{
				ExtensionObject[]? exObjArr = val as ExtensionObject[];
				if (exObjArr is null) continue;
				var innerStructs = new List<OpcNode>();

				foreach (var exObj in exObjArr)
				{
					//Write the body of the ExtensionObject into the byte-array
					readBinaryData = (byte[])exObj.Body;

					IEnumerable<OpcNode> Tags = OpcXmlParser.InsertValuesInStruct(varList, readBinaryData);

					//Check for data types and parse byte array
					innerStructs.Add(new() { InnerNodes = Tags });
				}
				result.InnerNodes = innerStructs;
			}
		}

		//return result as List<string[]> (string[0]=tag name; string[1]=tag value; string[2]=tag data type
		return result;
	}

	/// <summary>Get information about a method's input and output arguments</summary>
	/// <param name="nodeIdString">The node Id of a method as strings</param>
	/// <returns>Argument informations as strings</returns>
	/// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
	public IEnumerable<OpcMethodArguments> GetMethodArguments(IOpcClient client, string nodeIdString)
	{
		//Return input argument node informations
		//Argument[0] = argument type (input or output);
		//Argument[1] = argument name
		//Argument[2] = argument value
		//Argument[3] = argument data type
		List<OpcMethodArguments> arguments = new();
		//Create node id object by node id string
		NodeId nodeId = new(nodeIdString);

		//Check if node is method
		Node methodNode = GetNode(client, nodeIdString);
		if (methodNode.NodeClass != NodeClass.Method)
		{
			//Not method; return null
			return null;
		}

		//We need to browse for property (input and output arguments)
		//Create a collection for the browse results
		ReferenceDescriptionCollection referenceDescriptionCollection;
		//Browse from starting point for properties (input and output)
		referenceDescriptionCollection = ExtractReferenceCollection(client, methodNode.NodeId);

		//Check if arguments exist
		if (referenceDescriptionCollection?.Count <= 0) return null;
		var refList = referenceDescriptionCollection?.Where(s => s.DisplayName.Text == "InputArguments" || s.DisplayName.Text == "OutputArguments" && s.NodeClass == NodeClass.Variable);
		List<string> tempEncodebleCollection = new();
		foreach (var refDesc in refList)
		{
			//Read the input/output arguments
			object result = ReadNodeClassValue<object>(client, refDesc.NodeId.ToString());

			//Extract arguments
			if (result is null) continue;


			//Cast object to ExtensionObject because input and output arguments are always extension objects
			if (result is ExtensionObject encodeable)
			{
				tempEncodebleCollection.Add(encodeable.ToString());
				if (tempEncodebleCollection.Count == 4)
				{
					OpcMethodArgumentType type = (OpcMethodArgumentType)Enum.Parse(typeof(OpcMethodArgumentType), tempEncodebleCollection[0]);
					arguments.Add(new(type, tempEncodebleCollection[1], tempEncodebleCollection[2], tempEncodebleCollection[3]));
					tempEncodebleCollection = new();
				}
				continue;
			}
			ExtensionObject[]? exObjArr = result as ExtensionObject[];
			foreach (ExtensionObject exOb in exObjArr)
			{
				Argument arg = exOb.Body as Argument;
				string[] argumentInfos = new string[4];
				// Set type: input or output
				OpcMethodArgumentType type = (OpcMethodArgumentType)Enum.Parse(typeof(OpcMethodArgumentType), refDesc.DisplayName.Text);
				string dataType = "undefined";
				//Set argument data type (no array)
				if (arg.ArrayDimensions.Count == 0)
				{
					Node node = GetNode(client, arg.DataType.ToString());
					dataType = node.DisplayName.ToString();
				}
				// Data type is array
				else if (arg.ArrayDimensions.Count == 1)
				{
					Node node = GetNode(client, arg.DataType.ToString());
					dataType = node.DisplayName.ToString() + "[" + arg.ArrayDimensions[0].ToString() + "]";
				}

				arguments.Add(new(type, arg.Name, arg.Value, dataType));

			}

		}

		return arguments;

	}


	private string GetTypeDictionary(string nodeIdString, IOpcClient client, out string parseString)
	{
		//Read the desired node first and chekc if it's a variable
		Node node = client.Session.ReadNode(nodeIdString);
		if (node.NodeClass != NodeClass.Variable)
		{
			throw new WrongNodeClassException($"Incorrect node class for '{nodeIdString}'. Current class is {node.NodeClass}");
		}
		//Get the node id of node's data type
		VariableNode variableNode = (VariableNode)node.DataLock;
		NodeId nodeId = new(variableNode.DataType.Identifier, variableNode.DataType.NamespaceIndex);

		//Browse for HasEncoding
		client.Session.Browse(null, null, nodeId, 0u, BrowseDirection.Forward, ReferenceTypeIds.HasEncoding, true, 0, out byte[] continuationPoint, out ReferenceDescriptionCollection refDescCol);

		//Check For found reference
		if (refDescCol.Count == 0)
		{
			throw new WrongReferencesException($"Node '{nodeIdString}' has no inner nodes.");
		}

		//Check for HasEncoding reference with name "Default Binary"
		bool dataTypeFound = false;
		foreach (ReferenceDescription refDesc in refDescCol)
		{
			if (refDesc.DisplayName.Text == "Default Binary")
			{
				nodeId = new NodeId(refDesc.NodeId.Identifier, refDesc.NodeId.NamespaceIndex);
				dataTypeFound = true;
			}
			else if (!dataTypeFound)
			{
				throw new WrongReferencesException($"Node '{nodeIdString}' has found not reference with name 'Default Binary'.");
			}
		}

		//Browse for HasDescription
		refDescCol = null;
		client.Session.Browse(null, null, nodeId, 0u, BrowseDirection.Forward, ReferenceTypeIds.HasDescription, true, 0, out continuationPoint, out refDescCol);

		//Check For found reference
		if (refDescCol.Count == 0)
		{
			throw new WrongReferencesException("No data type description found in address space.");
		}

		//Read from node id of the found description to get a value to parse for later on
		nodeId = new NodeId(refDescCol[0].NodeId.Identifier, refDescCol[0].NodeId.NamespaceIndex);
		DataValue resultValue = client.Session.ReadValue(nodeId);
		parseString = resultValue.Value.ToString();

		//Browse for ComponentOf from last browsing result inversly
		refDescCol = null;
		client.Session.Browse(null, null, nodeId, 0u, BrowseDirection.Inverse, ReferenceTypeIds.HasComponent, true, 0, out continuationPoint, out refDescCol);

		//Check if reference was found
		if (refDescCol.Count == 0)
		{
			throw new WrongReferencesException("Data type isn't a component of parent type in address space. Can't continue decoding.");
		}

		//Read from node id of the found HasCompoment reference to get a XML file (as HEX string) containing struct/UDT information

		nodeId = new NodeId(refDescCol[0].NodeId.Identifier, refDescCol[0].NodeId.NamespaceIndex);
		resultValue = client.Session.ReadValue(nodeId);

		//Convert the HEX string to ASCII string
		string resultXml = Encoding.ASCII.GetString((byte[])resultValue.Value);

		//Return the dictionary as ASCII string
		return resultXml;
	}

	#region Namespace

	/// <summary>Returns a list of all namespace uris.</summary>
	/// <returns>The name space array</returns>
	public IEnumerable<string> GetNamespaceArray(IOpcClient client)
	{
		List<string> namespaceArray = new();

		//Read all namespace uris and add the uri to the list
		for (uint i = 0; i < client.Session.NamespaceUris.Count; i++)
		{
			namespaceArray.Add(client.Session.NamespaceUris.GetString(i));
		}

		return namespaceArray;
	}

	/// <summary>Returns the index of the specified namespace uri.</summary>
	/// <param name="uri">The namespace uri</param>
	/// <returns>The namespace index</returns>
	public uint GetNamespaceIndex(IOpcClient client, string uri)
	{
		//Get the namespace index of the specified namespace uri
		int namespaceIndex = client.Session.NamespaceUris.GetIndex(uri);
		//If the namespace uri doesn't exist, namespace index is -1
		if (namespaceIndex < 0) throw new NotFoundNamespaceException("Namespace doesn't exist");
		return (uint)namespaceIndex;
	}

	/// <summary>Returns the namespace uri at the specified index.</summary>
	/// <param name="index">the namespace index</param>
	/// <returns>The namespace uri</returns>
	public string GetNamespaceUri(IOpcClient client, uint index)
	{
		//Check the length of namespace array
		if (client.Session.NamespaceUris.Count <= index) throw new NotFoundNamespaceException("Index is out of range");
		//Get the uri for the namespace index
		return client.Session.NamespaceUris.GetString(index);
	}

	#endregion Namespace

}
