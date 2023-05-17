using MediatR;
using Opc.Ua;
using OpcUa.Application.Contratcs.Client;
using OpcUa.Domain.Contracts.Client;

namespace OpcUa.Application.Client.Commands.Connect;

public record CreateClientConnectionByUriCommand(string uri) : IRequest<IOpcClient> { }

public class CreateClientConnectionByUriCommandHandler : IRequestHandler<CreateClientConnectionByUriCommand, IOpcClient>
{
	private readonly IOpcConnector connector;

	public CreateClientConnectionByUriCommandHandler(IOpcConnector connector)
	{
		this.connector = connector;
	}
	public async Task<IOpcClient> Handle(CreateClientConnectionByUriCommand request, CancellationToken cancellationToken)
	{
		IOpcClient client;
		var endpoint = await TryGetEndpointAsync(request.uri);
		var client = await connector.CreateConnection(endpoint, false);
		return client
	}
	public async Task TryConnect(string url)
	{

	}
	private async Task<EndpointDescription> TryGetEndpointAsync(string url, MessageSecurityMode securityMode = MessageSecurityMode.None)
	{

		ApplicationDescriptionCollection servers = _client.FindServers(url) ?? throw new ServerOpcNullException($"Servers on url {url} not founded.");
		string firstServerUrl = servers[0].DiscoveryUrls[0];
		EndpointDescriptionCollection endpoints = _client.GetEndpoints(firstServerUrl);

		var endpoint = endpoints.FirstOrDefault(s => s.SecurityMode == securityMode) ?? throw new EndpointOpcNullException($"Servers on url {url} not founded.");
		return endpoint;
	}
}
