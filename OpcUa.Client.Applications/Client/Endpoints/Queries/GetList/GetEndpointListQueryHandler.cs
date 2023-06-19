using FluentResults;
using MediatR;
using Opc.Ua;
using OpcUa.Applications.Errors;

namespace OpcUa.Client.Applications.Client.Endpoints.Queries.GetList;

public class GetEndpointListQueryHandler : IRequestHandler<GetEndpointListQuery, Result<EndpointDescriptionCollection>>
{
    public async Task<Result<EndpointDescriptionCollection>> Handle(GetEndpointListQuery request, CancellationToken cancellationToken)
    {
        DiscoveryClient client = DiscoveryClient.Create(request.Uri);
        EndpointDescriptionCollection endpoints;
        try
        {
            endpoints = client.GetEndpoints(null);
            client.Close();
            client.Dispose();
            return Result.Ok(endpoints);
        }
        catch (Exception e)
        {
            client.Close();
            client.Dispose();
            return Result.Fail(new EndpointsNotFoundedError(request.Uri.AbsolutePath, e));
        }

    }
}
