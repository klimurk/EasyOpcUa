using System.Security.Cryptography.X509Certificates;

namespace OpcUa.Domain.Basics;

public class OpcNode
{
	public string Name { get; set; }
	public string DataType { get; set; } = "Undefined";
	public dynamic Value { get; set; }
	public IEnumerable<OpcNode> InnerNodes { get; set; } = new List<OpcNode>();

}