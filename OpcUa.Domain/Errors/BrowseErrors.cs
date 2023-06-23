using FluentResults;
using Opc.Ua;
using OpcUa.Domain.Contracts.Client;
using System.Text;

namespace OpcUa.Domain.Errors;


//public class BrowseNodeError : Error
//{
//    public BrowseNodeError(IOpcClient client, Node node)
//        : base(BuildError(client, node)) =>
//        Metadata.Add(nameof(ErrorCodes.BrowseNodeCode), ErrorCodes.BrowseNodeCode);

//    private static string BuildError(IOpcClient client, Node node)
//    {
//        StringBuilder sb = new($"Node {node} cannot read inner references in client with url {client.Session.Endpoint.EndpointUrl}.");
//        return sb.ToString();
//    }
//}
//public class BrowseReferenceError : Error
//{
//    public BrowseReferenceError(IOpcClient client, ReferenceDescription node)
//        : base(BuildError(client, node)) =>
//        Metadata.Add(nameof(ErrorCodes.BrowseNodeCode), ErrorCodes.BrowseNodeCode);

//    private static string BuildError(IOpcClient client, ReferenceDescription node)
//    {
//        StringBuilder sb = new($"Node {node} cannot read inner references in client with url {client.Session.Endpoint.EndpointUrl}.");
//        return sb.ToString();
//    }
//}
//public class BrowsingRootOpcServerError : Error
//{
//    public BrowsingRootOpcServerError(IOpcClient client)
//        : base(BuildError(client)) =>
//        Metadata.Add(nameof(ErrorCodes.BrowseRootCode), ErrorCodes.BrowseRootCode);

//    private static string BuildError(IOpcClient client)
//    {
//        StringBuilder sb = new($"Cannot read inner references in client with url {client.Session.Endpoint.EndpointUrl}.");
//        return sb.ToString();
//    }
//}
