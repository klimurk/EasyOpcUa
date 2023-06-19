using FluentResults;
using MediatR;
using OpcUa.Domain;
using OpcUa.Domain.Contracts.Client;

namespace OpcUa.Client.Applications.Client.Structs.Queries.GetStructValues;

public record ReadStructQuery(IOpcClient client, string nodeId) : IRequest<Result<object>> { }
