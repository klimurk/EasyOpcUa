using MediatR;
using Opc.Ua;
using OpcUa.Application.Client.Queries.Values.List.NodeIds;
using OpcUa.Domain.Contracts.Client;

namespace OpcUa.Application.Client.Queries.Values.List.Nodes;

public record ReadListNodeValueQuery(IOpcClient client, IEnumerable<Node> nodes, Type ExpectedType = null) : IRequest<OpcReadListResult> { }
