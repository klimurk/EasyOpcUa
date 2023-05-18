using MediatR;
using Opc.Ua;
using OpcUa.Application.Client.Queries.Browsings.Root;
using OpcUa.Domain.Contracts.Client;

namespace OpcUa.Application.Client.Queries.Nodes;

/// <param name="nodeIdString">The node Id as string</param>
/// <param name="client">Client you want to read</param>
public record GetNodeQuery(IOpcClient client, string nodeIdString) : IRequest<Node?>
{
}
