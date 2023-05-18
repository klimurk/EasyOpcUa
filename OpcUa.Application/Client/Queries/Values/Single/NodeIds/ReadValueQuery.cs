using MediatR;
using OpcUa.Domain.Contracts.Client;

namespace OpcUa.Application.Client.Queries.Values.Single.NodeIds;

public record ReadValueQuery(IOpcClient client, string nodeId, Type ExpectedType = null) : IRequest<object> { }
