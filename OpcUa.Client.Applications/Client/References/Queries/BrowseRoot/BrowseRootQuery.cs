using FluentResults;
using MediatR;
using Opc.Ua;
using OpcUa.Client.Applications.Queries.Browsings.References;
using OpcUa.Domain.Contracts.Client;
using System.Xml.Linq;

namespace OpcUa.Client.Applications.Client.References.Queries.BrowseRoot;

/// <param name="Client">Client you want to browse</param>
public record BrowseRootQuery(IOpcClient Client) : IRequest<Result<ReferenceDescriptionCollection>> { }


