using FluentResults;
using Opc.Ua;
using OpcUa.Domain.Contracts.Client;
using System.Text;

namespace OpcUa.Domain.Errors;

//public class ReadNodeError : Error
//{
//    public ReadNodeError(IOpcClient client, NodeId node)
//        : base(BuildError(client, node, e)) =>
//        Metadata.Add(nameof(ErrorCodes.ReadNodeCode), ErrorCodes.ReadNodeCode);

//    private static string BuildError(IOpcClient client, NodeId node)
//    {
//        StringBuilder sb = new StringBuilder($"Cannot read node {node} references in client with url {client.Session.Endpoint.EndpointUrl}.");
//        return sb.ToString();
//    }


//}
