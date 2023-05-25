using MediatR;
using Opc.Ua;
using OpcUa.Domain.Contracts.Client;

namespace OpcUa.Application.Client.Queries.Nodes;

/// <param name="NodeId">The node Id as string</param>
/// <param name="Client">Client you want to read</param>
public record GetNodeQuery(IOpcClient Client, string NodeId) : IRequest<Node?>
{
}
