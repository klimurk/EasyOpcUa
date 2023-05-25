using Opc.Ua;

namespace OpcUa.Domain;

public class OpcNode
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public BuiltInType DataType { get; set; }
	public DataValue Value { get; set; }
	public ICollection<OpcNode> InnerNodes { get; set; } = new List<OpcNode>();

}