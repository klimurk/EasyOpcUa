using FluentResults;
using MediatR;
using Opc.Ua;
using OpcUa.Domain.Errors;
using OpcUa.Domain.Errors;

namespace OpcUa.Client.Applications.Client.Servers.Queries.GetList;

public class GetServerListQueryHandler : IRequestHandler<GetServerListQuery, Result<ApplicationDescriptionCollection>>
{
    public async Task<Result<ApplicationDescriptionCollection>> Handle(GetServerListQuery request, CancellationToken cancellationToken)
    {
        DiscoveryClient client = DiscoveryClient.Create(request.Uri);
        try
        {
            ApplicationDescriptionCollection servers = client.FindServers(serverUris:null);
            client.Close();
            client.Dispose();
            return servers;
        }
        catch (Exception e)
        {
            client.Close();
            client.Dispose();
            return Result.Fail(DomainErrors.Opc.Client.Server.NotFoundError.CausedBy(e));
        }

    }
}
