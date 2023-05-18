using MediatR;
using Opc.Ua;
using OpcUa.Application.Client.Exceptions;

namespace OpcUa.Application.Client.Queries.Browsings.References;


/// <summary>Browses a node ID provided by a ReferenceDescription</summary>
/// <returns>ReferenceDescriptionCollection of found nodes</returns>
/// <exception cref="BrowsingException">Throws and forwards exception while have error while browsing.</exception>
public class BrowseReferenceQueryHandler() : IRequestHandler<BrowseReferenceQuery, ReferenceDescriptionCollection?>
{
	public async Task<ReferenceDescriptionCollection?> Handle(BrowseReferenceQuery request, CancellationToken cancellationToken)
	{
		//Create a NodeId using the selected ReferenceDescription as browsing starting point
		NodeId nodeId = ExpandedNodeId.ToNodeId(request.RefDesc.NodeId, null);
		ReferenceDescriptionCollection referenceDescriptionCollection;
		try
		{
			//Browse from starting point for all object types

			request.Client.Session.Browse(null, view: null, nodeId, 0u, BrowseDirection.Forward, ReferenceTypeIds.HierarchicalReferences, true, 0, out byte[] continuationPoint, out referenceDescriptionCollection);
			while (continuationPoint is not null && continuationPoint?.Length > 0)
			{
				request.Client.Session.BrowseNext(null, false, continuationPoint, out byte[] revisedContinuationPoint, out ReferenceDescriptionCollection nextReferenceDescriptionCollection);
				continuationPoint = revisedContinuationPoint;
				referenceDescriptionCollection.AddRange(nextReferenceDescriptionCollection);
			}
		}
		catch (Exception e)
		{
			throw new BrowsingException("Error while browsing", e.InnerException);
		}
		return referenceDescriptionCollection;
	}
}


