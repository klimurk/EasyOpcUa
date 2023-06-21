using FluentResults;
using Opc.Ua;
using OpcUa.Domain.Contracts.Client;
using System.Text;

namespace OpcUa.Domain.Errors;


public class BrowseNodeError : Error
{
    public BrowseNodeError(IOpcClient client, Node node, Exception e = default)
        : base(BuildError(client, node, e)) =>
        Metadata.Add(nameof(ErrorCodes.BrowseNodeCode), ErrorCodes.BrowseNodeCode);

    private static string BuildError(IOpcClient client, Node node, Exception e)
    {
        StringBuilder sb = new($"Node {node} cannot read inner references in client with url {client.Session.Endpoint.EndpointUrl}.");
        if (e != default) sb.AppendLine($"Exception {e}");
        return sb.ToString();
    }
}
public class BrowseReferenceError : Error
{
    public BrowseReferenceError(IOpcClient client, ReferenceDescription node, Exception e = default)
        : base(BuildError(client, node, e)) =>
        Metadata.Add(nameof(ErrorCodes.BrowseNodeCode), ErrorCodes.BrowseNodeCode);

    private static string BuildError(IOpcClient client, ReferenceDescription node, Exception e)
    {
        StringBuilder sb = new($"Node {node} cannot read inner references in client with url {client.Session.Endpoint.EndpointUrl}.");
        if(e != default) sb.AppendLine($"Exception {e.Message}");
        return sb.ToString();
    }
}
public class BrowsingRootOpcServerError : Error
{
    public BrowsingRootOpcServerError(IOpcClient client, Exception e = default)
        : base(BuildError(client, e)) =>
        Metadata.Add(nameof(ErrorCodes.BrowseRootCode), ErrorCodes.BrowseRootCode);

    private static string BuildError(IOpcClient client, Exception e)
    {
        StringBuilder sb = new($"Cannot read inner references in client with url {client.Session.Endpoint.EndpointUrl}.");
        if (e != default) sb.AppendLine($"Exception {e.Message}");
        return sb.ToString();
    }
}
