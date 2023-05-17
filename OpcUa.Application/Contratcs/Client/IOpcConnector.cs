using Opc.Ua;
using OpcUa.Domain.Contracts.Client;

namespace OpcUa.Application.Contratcs.Client;

public interface IOpcConnector
{
	Task<IOpcClient> CreateConnection(EndpointDescription endpointDescription, bool userAuth, string userName = null, string password = null, string sessionName = "MySession", uint sessionTimeout = 60000);
	Task<IOpcClient> CreateConnection(string url, string secPolicy, MessageSecurityMode msgSecMode, bool userAuth, string userName, string password, string sessionName = "MySession", uint sessionTimeout = 60000);
	void Disconnect(IOpcClient client);
}
