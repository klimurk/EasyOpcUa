using FluentResults;
using MediatR;
using Opc.Ua;
using OpcUa.Applications.Errors;

namespace OpcUa.Client.Applications.Client.References.Queries.BrowseRoot;

public class BrowseRootQueryAsyncHandler : IRequestHandler<BrowseRootQuery, Result<ReferenceDescriptionCollection>>
{

    /// <summary>Browses the root folder of an OPC UA server.</summary>
    /// <returns>ReferenceDescriptionCollection of found nodes</returns>
    /// <exception cref="OpcConnectionException">Throws and forwards exception while have error while browsing.</exception>
    public async Task<Result<ReferenceDescriptionCollection>> Handle(BrowseRootQuery request, CancellationToken cancellationToken)
    {

        RequestHeader requestHeader = null;
        ViewDescription viewDescription = null;
        uint maxResultsToReturn = 0;
        uint nodeClassMask = (uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method;
        bool IncludeSubtypes = true;
        BrowseResponse response;
        try
        {
            //Browse the RootFolder for variables, objects and methods
            response = await request.Client.Session.BrowseAsync(requestHeader, viewDescription, maxResultsToReturn, null, cancellationToken);
        }
        catch (Exception e)
        {
            return Result.Fail(new BrowsingRootOpcServerError(request.Client, e));
        }
        var collection = response.Results.Select(s => s.References).First(s => s.Count > 0);
        return Result.Ok(collection);
    }
}

