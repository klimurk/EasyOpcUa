using FluentResults;
using MediatR;
using Opc.Ua;
using OpcUa.Domain.Contracts.Client;

namespace OpcUa.Client.Applications.Client.Nodes.Queries.GetValue;

public record ReadNodeValueQuery(IOpcClient client, Node nodes, Type ExpectedType = null) : IRequest<Result<object>> { }
