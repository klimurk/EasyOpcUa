using MediatR;
using OpcUa.Domain.Contracts.Client;

namespace OpcUa.Application.Client.Queries.Structs;

public record ReadStructQuery(IOpcClient client, string nodeId, Type ExpectedType = null) : IRequest<object> { }
