using MediatR;
using Opc.Ua;
using OpcUa.Application.Client.Exceptions;

namespace OpcUa.Application.Client.Queries.Browsings.Root;

/// <summary>Browses the root folder of an OPC UA server.</summary>
/// <returns>ReferenceDescriptionCollection of found nodes</returns>
/// <exception cref="OpcConnectionException">Throws and forwards exception while have error while browsing.</exception>
public class BrowseRootQueryHandler : IRequestHandler<BrowseRootQuery, ReferenceDescriptionCollection>
{
    public async Task<ReferenceDescriptionCollection> Handle(BrowseRootQuery request, CancellationToken cancellationToken)
    {
        RequestHeader requestHeader = null;
        ViewDescription viewDescription = null;
        uint maxResultsToReturn = 0;
        uint nodeClassMask = (uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method;
        bool IncludeSubtypes = true;
        ReferenceDescriptionCollection referenceDescriptionCollection;
        try
        {
            //Browse the RootFolder for variables, objects and methods
            request.Client.Session.Browse(requestHeader, viewDescription, ObjectIds.RootFolder,
                maxResultsToReturn, BrowseDirection.Forward, ReferenceTypeIds.HierarchicalReferences, IncludeSubtypes, nodeClassMask,
                out byte[] continuationPoint, out referenceDescriptionCollection);
        }
        catch (Exception e)
        {
            // exception
            throw new OpcConnectionException("Error while try browsing", e.InnerException);
        }
        return referenceDescriptionCollection;
    }
}

