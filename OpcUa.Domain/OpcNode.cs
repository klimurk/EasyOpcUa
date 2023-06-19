using Opc.Ua;

namespace OpcUa.Domain;

public class OpcNode
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public BuiltInType DataType { get; set; }
	public DataValue DataValue { get; set; }
	public object Value => DataValue.Value;
	public ICollection<OpcNode> InnerNodes { get; set; } = new List<OpcNode>();

}