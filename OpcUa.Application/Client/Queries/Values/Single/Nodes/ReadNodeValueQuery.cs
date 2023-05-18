using MediatR;
using Opc.Ua;
using OpcUa.Domain.Contracts.Client;

namespace OpcUa.Application.Client.Queries.Values.List.Nodes;

public record ReadNodeValueQuery(IOpcClient client, Node nodes, Type ExpectedType = null) : IRequest<object> { }
