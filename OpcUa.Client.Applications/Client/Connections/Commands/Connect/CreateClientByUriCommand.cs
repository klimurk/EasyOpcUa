using FluentResults;
using MediatR;
using Opc.Ua;
using Opc.Ua.Client;
using OpcUa.Domain.Errors;
using OpcUa.Client.Applications.Helpers;
using OpcUa.Domain;
using OpcUa.Domain.Contracts.Client;
//using Opc.Ua.Server;
using Session = Opc.Ua.Client.Session;

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
        var applicationConfig = request.clientAppConfig is null ?
            await new ApplicationConfigurationFactory().CreateAsync() :
            request.clientAppConfig;

        try
        {
            //Update certificate store before connection attempt
            await applicationConfig.CertificateValidator.Update(applicationConfig);
        }
        catch (Exception e)
        {
            return Result.Fail(DomainErrors.Opc.Client.Certificates.ValidationError.WithMetadata(nameof(applicationConfig), applicationConfig).CausedBy(e));
        }

        //Create EndPoint configuration
        EndpointConfiguration endpointConfiguration = EndpointConfiguration.Create(applicationConfig);
        //Connect to server and get endpoints
        ConfiguredEndpoint endpoint = new(collection: null, request.EndpointDescription, endpointConfiguration);
        Session session;
        try
        {
            session = await Session.Create(
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
            return Result.Fail(DomainErrors.Opc.Client.Server.NotFoundError.CausedBy(e));
        }
        return new OpcUaClient(applicationConfig, session);
    }

}
