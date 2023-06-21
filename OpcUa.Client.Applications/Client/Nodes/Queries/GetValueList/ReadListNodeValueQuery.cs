using FluentResults;
using MediatR;
using Opc.Ua;
using OpcUa.Domain.Contracts.Client;

namespace OpcUa.Client.Applications.Client.Nodes.Queries.GetValueList;

public record ReadListNodeValueQuery(IOpcClient client, IEnumerable<Node> nodes, Type ExpectedType = null) : IRequest<Result<DataValueCollection>> { }
