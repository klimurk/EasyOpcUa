using FluentResults;
using MediatR;
using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcUa.Client.Applications.Client.Endpoints.Queries.GetList;

public record GetEndpointListQuery(Uri Uri) : IRequest<Result<EndpointDescriptionCollection>>;
