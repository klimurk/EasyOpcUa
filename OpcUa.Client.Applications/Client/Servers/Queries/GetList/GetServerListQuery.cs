using FluentResults;
using MediatR;
using Opc.Ua;

namespace OpcUa.Client.Applications.Client.Servers.Queries.GetList;

public record GetServerListQuery(Uri Uri) : IRequest<Result<ApplicationDescriptionCollection>>;
