using MediatR;
using Opc.Ua;
using OpcUa.Application.Client.Exceptions;
using OpcUa.Application.Helpers;
using OpcUa.Domain;
using OpcUa.Domain.Contracts.Client;
using System.Text;

namespace OpcUa.Application.Client.Queries.Structs;

public class ReadStructQueryHandler : IRequestHandler<ReadStructQuery, object>
{
	public async Task<object> Handle(ReadStructQuery request, CancellationToken cancellationToken)
	{
		//Read the struct
		IList<NodeId> nodeIds = new List<NodeId>() { new(request.nodeId) };
		List<ServiceResult> serviceResults = new();
		List<object> values = new();
		IList<Type> types = new List<Type>() { null };
		try
		{
			request.client.Session.ReadValues(nodeIds, types, out values, out serviceResults);
		}
		catch (Exception e)
		{
			//Handle Exception here
			throw e;
		}
		//Check result codes
		foreach (ServiceResult? errorResult in serviceResults.Where(s => s.StatusCode != StatusCodes.Good))
		{
			throw new ReadNodeException(errorResult.ToString());
		}


		//Define result list to return var name and var value
		var result = new OpcNode();

		//Get the type dictionary of desired struct/UDT and the name of desired var to parse
		string parseString;
		string xmlString = GetTypeDictionary(request.nodeId, request.client, out parseString);
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
	/// <summary>Browses for the desired type dictonary to parse for containing data types</summary>
	/// <param name="nodeIdString">The node Id string</param>
	/// <param name="client">The current session to browse in</param>
	/// <param name="parseString">The name of the var to parse for inside of dictionary</param>
	/// <returns>The dictionary as ASCII string</returns>
	/// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
	private static string GetTypeDictionary(string nodeIdString, IOpcClient client, out string parseString)
	{
		//Read the desired node first and chekc if it's a variable
		Node node = client.Session.ReadNode(nodeIdString);
		if (node.NodeClass == NodeClass.Variable)
		{
			//Get the node id of node's data type
			VariableNode variableNode = (VariableNode)node.DataLock;
			NodeId nodeId = new(variableNode.DataType.Identifier, variableNode.DataType.NamespaceIndex);

			//Browse for HasEncoding
			ReferenceDescriptionCollection refDescCol;
			byte[] continuationPoint;
			client.Session.Browse(null, null, nodeId, 0u, BrowseDirection.Forward, ReferenceTypeIds.HasEncoding, true, 0, out continuationPoint, out refDescCol);

			//Check For found reference
			if (refDescCol.Count == 0)
			{
				Exception ex = new("No data type to encode. Could be a build-in data type you want to read.");
				throw ex;
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
					Exception ex = new("No default binary data type found.");
					throw ex;
				}
			}

			//Browse for HasDescription
			refDescCol = null;
			client.Session.Browse(null, null, nodeId, 0u, BrowseDirection.Forward, ReferenceTypeIds.HasDescription, true, 0, out continuationPoint, out refDescCol);

			//Check For found reference
			if (refDescCol.Count == 0)
			{
				Exception ex = new("No data type description found in address space.");
				throw ex;
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
				Exception ex = new("Data type isn't a component of parent type in address space. Can't continue decoding.");
				throw ex;
			}

			//Read from node id of the found HasCompoment reference to get a XML file (as HEX string) containing struct/UDT information

			nodeId = new NodeId(refDescCol[0].NodeId.Identifier, refDescCol[0].NodeId.NamespaceIndex);
			resultValue = client.Session.ReadValue(nodeId);

			//Convert the HEX string to ASCII string
			string xmlString = Encoding.ASCII.GetString((byte[])resultValue.Value);

			//Return the dictionary as ASCII string
			return xmlString;
		}
		{
			Exception ex = new("No variable data type found");
			throw ex;
		}
	}

}
