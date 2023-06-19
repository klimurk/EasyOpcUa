using FluentResults;
using MediatR;
using Opc.Ua;
using OpcUa.Applications.Errors;

namespace OpcUa.Client.Applications.Client.Servers.Queries.GetList;

public class GetServerListQueryHandler : IRequestHandler<GetServerListQuery, Result<ApplicationDescriptionCollection>>
{
    public async Task<Result<ApplicationDescriptionCollection>> Handle(GetServerListQuery request, CancellationToken cancellationToken)
    {
        DiscoveryClient client = DiscoveryClient.Create(request.Uri);
        try
        {
            ApplicationDescriptionCollection servers = client.FindServers(null);
            client.Close();
            client.Dispose();
            return servers;
        }
        catch (Exception e)
        {
            client.Close();
            client.Dispose();
            return Result.Fail(new ServersNotFoundedError("All", e));
        }

    }
}
