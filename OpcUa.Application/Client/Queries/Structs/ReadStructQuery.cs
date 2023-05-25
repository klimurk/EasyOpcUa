using MediatR;
using OpcUa.Domain;
using OpcUa.Domain.Contracts.Client;

namespace OpcUa.Application.Client.Queries.Structs;

public record ReadStructQuery(IOpcClient client, string nodeId) : IRequest<OpcNode> { }
