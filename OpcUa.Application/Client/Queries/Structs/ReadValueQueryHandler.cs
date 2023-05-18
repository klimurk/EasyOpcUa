using MediatR;
using Opc.Ua;
using OpcUa.Application.Client.Exceptions;

namespace OpcUa.Application.Client.Queries.Structs;

public class ReadStructQueryHandler : IRequestHandler<ReadStructQuery, object>
{
	public async Task<object> Handle(ReadStructQuery request, CancellationToken cancellationToken)
	{
		object result;
		try
		{
			result = request.client.Session.ReadValue(new NodeId(request.nodeId), request.ExpectedType);
		}
		catch (Exception e)
		{
			throw new BrowsingException("Error connection while read values", e.InnerException);
		}
		return result;
	}
}
