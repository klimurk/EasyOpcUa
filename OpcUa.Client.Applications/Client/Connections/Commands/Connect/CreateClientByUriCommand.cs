using FluentResults;
using MediatR;
using Opc.Ua;
using Opc.Ua.Client;
using OpcUa.Applications.Errors;
using OpcUa.Client.Applications.Helpers;
using OpcUa.Domain;
using OpcUa.Domain.Contracts.Client;

namespace OpcUa.Client.Applications.Client.Connections.Commands.Connect;

public record CreateClientByUriCommand(
    EndpointDescription EndpointDescription,
    string SessionName,
    ApplicationConfiguration clientAppConfig = default,
    UserIdentity UserIdentity = default,
    uint SessionTimeout = 60000)
    : IRequest<Result<IOpcClient>>;

public class CreateClientByUriCommandHandler : IRequestHandler<CreateClientByUriCommand, Result<IOpcClient>>
{
    public async Task<Result<IOpcClient>> Handle(CreateClientByUriCommand request, CancellationToken cancellationToken)
    {
        var applicationConfig = request.clientAppConfig is null ? ApplicationConfigurationFactory.CreateClientConfiguration() : request.clientAppConfig;

        try
        {
            //Update certificate store before connection attempt
            await applicationConfig.CertificateValidator.Update(applicationConfig);
        }
        catch (Exception e)
        {
            return Result.Fail(new ClientCertificateValidationError(applicationConfig, e));
        }
        IOpcClient client = new OpcUaClient(applicationConfig);

        //Create EndPoint configuration
        EndpointConfiguration endpointConfiguration = EndpointConfiguration.Create(applicationConfig);
        //Connect to server and get endpoints
        ConfiguredEndpoint endpoint = new(collection: null, request.EndpointDescription, endpointConfiguration);
        try
        {
            client.Session = await Session.Create(
                applicationConfig,
                endpoint,
                updateBeforeConnect: true,
                request.SessionName,
                request.SessionTimeout,
                request.UserIdentity,
                preferredLocales: null
                );
        }
        catch (Exception e)
        {
            return Result.Fail(new ConnectServerError(request.EndpointDescription, request.UserIdentity, request.clientAppConfig, e));
        }
        return Result.Ok(client);
    }

}
