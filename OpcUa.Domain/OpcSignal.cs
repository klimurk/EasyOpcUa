using Opc.Ua;
using System.Reactive.Subjects;

namespace Woodnailer.Domain.Opc;

public class OpcSignal
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public string Address { get; set; }
	public Node Node { get; set; }
	public BehaviorSubject<DataValue> Data { get; protected set; } = new BehaviorSubject<DataValue>(new());
}
