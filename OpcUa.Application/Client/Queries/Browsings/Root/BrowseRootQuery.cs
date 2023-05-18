using MediatR;
using Opc.Ua;
using OpcUa.Application.Client.Queries.Browsings.References;
using OpcUa.Domain.Contracts.Client;
using System.Xml.Linq;

namespace OpcUa.Application.Client.Queries.Browsings.Root;

/// <param name="Client">Client you want to browse</param>
public record BrowseRootQuery(IOpcClient Client) : IRequest<ReferenceDescriptionCollection?> { }


