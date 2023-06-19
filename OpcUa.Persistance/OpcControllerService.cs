using MediatR;
using FluentResults;
using Opc.Ua;
using OpcUa.Domain.Contracts.Client;
using OpcUa.Client.Applications.Endpoints.Queries.GetList;
using OpcUa.Client.Applications.Client.Commands.Connect;
using OpcUa.Client.Applications.Servers.Queries.GetList;
using OpcUa.Persistance.Startup;

namespace OpcUa.Persistance;

public class OpcControllerService
{
	private readonly IMediator mediator;
    private readonly IAppConfig appConfig;

    public OpcControllerService(IMediator mediator, IAppConfig appConfig)
	{

		this.mediator = mediator;
        this.appConfig = appConfig;
    }
	public async void CheckList()
	{
		Uri uri = new(appConfig.PlcData.PlcAddress);
		Result<ApplicationDescriptionCollection> servers = await mediator.Send(new GetServerListQuery(uri));
		string server = servers.Value.First().DiscoveryUrls.First();
		Uri serverUri = new(server);
		var endpoints = await mediator.Send(new GetEndpointListQuery(serverUri));
		EndpointDescription? endpointDescription = endpoints.Value.Find(s => s.SecurityMode == MessageSecurityMode.None);
		Result<IOpcClient> client = await mediator.Send(new CreateClientByUriCommand(endpointDescription, "newSession"));

	}

}
