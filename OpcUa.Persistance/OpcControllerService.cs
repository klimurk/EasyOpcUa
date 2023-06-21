using MediatR;
using FluentResults;
using Opc.Ua;
using OpcUa.Domain.Contracts.Client;
using OpcUa.Persistance.Startup;
using OpcUa.Client.Applications.Client.Servers.Queries.GetList;
using OpcUa.Client.Applications.Client.Endpoints.Queries.GetList;
using OpcUa.Client.Applications.Client.Connections.Commands.Connect;
using Microsoft.Extensions.Options;
using OpcUa.Client.Applications.Client.Nodes.Queries.GetNode;
using OpcUa.Domain;
using OpcUa.Client.Applications.Client.Subscriptions.Commands.SubscribeNode;
using System.Linq;

namespace OpcUa.Persistance;

public class OpcControllerService
{
    private readonly IMediator mediator;
    private readonly IAppConfig appConfig;
    private IOpcClient _Client;
    private AutoResetEvent _AutoResetEvent = new(true);

    public OpcControllerService(IMediator mediator, IAppConfig appConfig)
    {

        this.mediator = mediator;
        this.appConfig = appConfig;
    }
    public async void CheckList()
    {
        _AutoResetEvent.WaitOne();
        Uri uri = new(appConfig.PlcData.PlcAddress);
        Result<ApplicationDescriptionCollection> serverResult;
        do
        {
            serverResult = await mediator.Send(new GetServerListQuery(uri));
        } while (serverResult.IsFailed);
        string server = serverResult.Value.First().DiscoveryUrls.First();
        Uri serverUri = new(server);
        var endpoints = await mediator.Send(new GetEndpointListQuery(serverUri));
        EndpointDescription? endpointDescription = endpoints.Value.Find(s => s.SecurityMode == MessageSecurityMode.None);
        Result<IOpcClient> clientResult = await mediator.Send(new CreateClientByUriCommand(endpointDescription, "newSession"));
        _Client = clientResult.Value;

        OpcInit opcInit = appConfig.Initialization.Opc.First();
        _Client.Name = opcInit.Name;
        _Client.CreateSubscription(10);
        var nodesResult = await mediator.Send(new GetNodeListQuery(_Client, opcInit.Signals.Select(s => s.Address)));
        var nodes = nodesResult.Value.Select(s => new OpcNode(s, opcInit.Signals.First(v => v.Address == s.NodeId.ToString()).Name));
        await mediator.Send(new SubscribeNodeListCommand(_Client, nodes, MonitoringMode.Reporting, 1));

        //foreach (OpcSignalInit nodeInit in opcInit.Signals)
        //{
        //    Result<Node> nodeResult = await mediator.Send(new GetNodeQuery(_Client, nodeInit.Address));
        //    Node node = nodeResult.Value;
        //    OpcNode myNode = new(node, nodeInit.Name);

        //}
        _AutoResetEvent.Set();
    }

}
