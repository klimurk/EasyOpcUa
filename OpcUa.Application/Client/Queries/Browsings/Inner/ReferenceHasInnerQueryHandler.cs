using MediatR;
using Opc.Ua;
using OpcUa.Application.Client.Exceptions;

namespace OpcUa.Application.Client.Queries.Browsings.Inner;

/// <summary>Browses a node has inner nodes</summary>
/// <returns>Bool</returns>
/// <exception cref="BrowsingException">Throws and forwards exception while have error while browsing.</exception>
public class ReferenceHasInnerQueryHandler : IRequestHandler<ReferenceHasInnerQuery, bool>
{
	public async Task<bool> Handle(ReferenceHasInnerQuery request, CancellationToken cancellationToken)
	{
		//Create a NodeId using the selected ReferenceDescription as browsing starting point
		NodeId nodeId = ExpandedNodeId.ToNodeId(request.node.NodeId, null);
		ReferenceDescriptionCollection referenceDescriptionCollection;
		try
		{
			//Browse from starting point for all object types
			request.Client.Session.Browse(null, null, nodeId, 0u, BrowseDirection.Forward, ReferenceTypeIds.HierarchicalReferences, true, 0, out byte[] continuationPoint, out referenceDescriptionCollection);
		}
		catch (Exception e)
		{
			//handle Exception here
			throw new BrowsingException("Error while browsing", e.InnerException);
		}
		return referenceDescriptionCollection?.Count > 0; ;
	}
}




