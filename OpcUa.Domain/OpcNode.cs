using Opc.Ua;
using shortid;
using System.Reactive.Subjects;

namespace OpcUa.Domain;

public class OpcNode
{
    public string Id { get; private set; } = ShortId.Generate(new(useNumbers: false, useSpecialCharacters: false, length: 8));
    public Node Node { get; private set; }
    public string Name { get; private set; }
    public BuiltInType DataType { get; set; }
    public ReplaySubject<DataValue> Data { get; private set; } = new(1);
    public ReplaySubject<object> Value { get; private set; } = new(1);
    public ICollection<OpcNode> InnerNodes { get; set; }
    public OpcNode(Node node, string customName = "")
    {
        Data.Subscribe(s => Value.OnNext(s.Value));
        Node = node;
        Name = customName == "" ? Node.DisplayName.Text : customName;
    }
}