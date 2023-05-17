using Opc.Ua;
using OpcUa.Persistance.Exceptions.Connection;

namespace OpcUa.Helpers;

internal class EndpointHelper
{
	public static EndpointDescription CreateEndpointDescription(string url, string secPolicy, MessageSecurityMode msgSecMode) => new()
	{
		EndpointUrl = url,
		SecurityPolicyUri = secPolicy,
		SecurityMode = msgSecMode,
		TransportProfileUri = Profiles.UaTcpTransport
	};
	/// <summary>Finds Servers based on a discovery url</summary>
	/// <param name="discoveryUrl">The discovery url</param>
	/// <returns>ApplicationDescriptionCollection containing found servers</returns>
	/// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
	public static ApplicationDescriptionCollection FindServers(string discoveryUrl)
	{
		//Create a URI using the discovery URL
		int countRepeat = 0;
		Uri uri = new(discoveryUrl);
		try
		{
			countRepeat++;
			ApplicationDescriptionCollection servers = GetServers(uri);
			return servers;
		}
		catch (ServersNotFoundException ex)
		{
			if (countRepeat >= 3) { throw ex; }
			return FindServers(discoveryUrl);
		}
	}
	private static ApplicationDescriptionCollection? GetServers(Uri uri)
	{
		ApplicationDescriptionCollection? servers = null;

		DiscoveryClient client = DiscoveryClient.Create(uri);
		servers = client.FindServers(null);
		client.Close();
		client.Dispose();

		return servers;
	}
}
