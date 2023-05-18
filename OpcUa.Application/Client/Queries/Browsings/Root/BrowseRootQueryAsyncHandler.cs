using MediatR;
using Opc.Ua;
using OpcUa.Application.Client.Exceptions;

namespace OpcUa.Application.Client.Queries.Browsings.Root;

public class BrowseRootQueryAsyncHandler : IRequestHandler<BrowseRootQuery, ReferenceDescriptionCollection?>
{

	/// <summary>Browses the root folder of an OPC UA server.</summary>
	/// <returns>ReferenceDescriptionCollection of found nodes</returns>
	/// <exception cref="BrowsingException">Throws and forwards exception while have error while browsing.</exception>
	public async Task<ReferenceDescriptionCollection?> Handle(BrowseRootQuery request, CancellationToken cancellationToken)
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
            throw new BrowsingException("Error while browsing",e.InnerException);
        }
        var collection = response.Results.Select(s => s.References).First(s => s.Count > 0) ;
        return collection;
    }
}


