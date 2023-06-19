using FluentResults;
using MediatR;
using OpcUa.Domain.Contracts.Client;

namespace OpcUa.Client.Applications.Client.NodeIds.Queries.GetValue;

public record ReadValueQuery(IOpcClient client, string nodeId, Type ExpectedType = null) : IRequest<Result<object>> { }
