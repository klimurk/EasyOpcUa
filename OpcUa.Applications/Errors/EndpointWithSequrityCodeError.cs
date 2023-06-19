using FluentResults;
using Opc.Ua;
using OpcUa.Domain.Contracts.Client;

namespace OpcUa.Applications.Errors;

public class EndpointWithSequrityCodeError : Error
{
	public EndpointWithSequrityCodeError(string uri, MessageSecurityMode messageSecurityMode)
		: base($"Endpoints on {uri} with sequrity {messageSecurityMode} not found.") =>
		Metadata.Add(nameof(ErrorCodes.EndpointSecurityCodeNotFoundCode), ErrorCodes.EndpointSecurityCodeNotFoundCode);
}
public class ReferenceHasInnerError : Error
{
	public ReferenceHasInnerError(IOpcClient client, Node node, Exception e = default)
		: base($"Node {node} cannot read inner references in client with url {client.Session.Endpoint.EndpointUrl}. Exception {e.Message}") =>
		Metadata.Add(nameof(ErrorCodes.ReferenceHasInnerErrorCode), ErrorCodes.ReferenceHasInnerErrorCode);
}
public class BrowseNodeError : Error
{
	public BrowseNodeError(IOpcClient client, Node node, Exception e = default)
		: base($"Node {node} cannot read inner references in client with url {client.Session.Endpoint.EndpointUrl}. Exception {e.Message}") =>
		Metadata.Add(nameof(ErrorCodes.BrowseNodeCode), ErrorCodes.BrowseNodeCode);
}
public class BrowseReferenceError : Error
{
	public BrowseReferenceError(IOpcClient client, ReferenceDescription node, Exception e = default)
		: base($"Node {node} cannot read inner references in client with url {client.Session.Endpoint.EndpointUrl}. Exception {e.Message}") =>
		Metadata.Add(nameof(ErrorCodes.BrowseNodeCode), ErrorCodes.BrowseNodeCode);
}
public class BrowsingRootOpcServerError : Error
{
	public BrowsingRootOpcServerError(IOpcClient client,  Exception e = default)
		: base($"Cannot read inner references in client with url {client.Session.Endpoint.EndpointUrl}. Exception {e.Message}") =>
		Metadata.Add(nameof(ErrorCodes.BrowseRootCode), ErrorCodes.BrowseRootCode);
}
public class ReadNodeError : Error
{
	public ReadNodeError(IOpcClient client, NodeId node, Exception e = default)
		: base($"Cannot read node {node} references in client with url {client.Session.Endpoint.EndpointUrl}. Exception {e.Message}") =>
		Metadata.Add(nameof(ErrorCodes.ReadNodeCode), ErrorCodes.ReadNodeCode);
}