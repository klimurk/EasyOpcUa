using MediatR;
using Opc.Ua;
using OpcUa.Application.Client.Exceptions;

namespace OpcUa.Application.Client.Queries.Nodes;

/// <summary>Reads a node by node Id</summary>
/// <returns>The read node</returns>
/// <exception cref="ReadNodeException">Throws and forwards exception while have problem with connection.</exception>
public class GetNodeQueryHandler : IRequestHandler<GetNodeQuery, Node?>
{
	public async Task<Node?> Handle(GetNodeQuery request, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(request.Client);
		string idString = request.NodeId.Trim();
		ArgumentException.ThrowIfNullOrEmpty(idString);
		Node result;
		try
		{
			result = request.Client.Session.ReadNode(new NodeId(idString));
		}
		catch (Exception e)
		{
			throw new ReadNodeException($"Cannot read node '{idString}' from session", e.InnerException);
		}
		return result;
	}
}

