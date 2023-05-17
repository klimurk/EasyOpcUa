using AutoMapper;
using MediatR;
using Opc.Ua;
using OpcUa.Application.Common.Mappings;

namespace OpcUa.Application.Opc;


public class OpcWriteNodeValue : IMapWith<WriteValue>
{
	public OpcWriteNodeValue(string NodeIdString, object Value) => (this.NodeIdString, this.Value) = (NodeIdString, Value);

	public string NodeIdString { get; private set; }
	public object Value { get; private set; }

	public void Mapping(Profile profile)
	{
		profile.CreateMap<OpcWriteNodeValue, WriteValue>()
			.ForMember(dto => dto.Value,
				opt => opt.MapFrom(node => new DataValue(new Variant(node.Value))))
			.ForMember(dto => dto.NodeId,
				opt => opt.MapFrom(node => new NodeId(NodeIdString)))
			.ForMember(dto => dto.AttributeId,
				opt => opt.MapFrom(node => Attributes.Value))
				;
	}
}

public record OpcWriteCommand(string address, object value) : IRequest<Task> { }

public class OpcWriteCommandHandler : IRequestHandler<OpcWriteCommand, Task>
{
	public Task<Task> Handle(OpcWriteCommand request, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}
}