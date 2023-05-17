using Opc.Ua.Client;
using Opc.Ua;
using OpcUa.Helpers;
using System.Text;
using OpcUa.Domain;
using OpcUa.Domain.Contracts.Client;
using OpcUa.Application.Contratcs.Client;

namespace Woodnailer.Application.Opc.Client;

public class OpcConnector : IOpcConnector
{
	#region Connect/Disconnect

	/// <summary>Establishes the connection to an OPC UA server and creates a session using a server url.</summary>
	/// <param name="url">The Url of the endpoint as string.</param>
	/// <param name="secPolicy">The security policy to use</param>
	/// <param name="msgSecMode">The message security mode to use</param>
	/// <param name="userAuth">Autheticate anonymous or with username and password</param>
	/// <param name="userName">The user name</param>
	/// <param name="password">The password</param>
	/// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>

	public async Task<IOpcClient> CreateConnection(string url, string secPolicy, MessageSecurityMode msgSecMode, bool userAuth, string userName, string password,
		string sessionName = "MySession", uint sessionTimeout = 60000)
	{
		EndpointDescription endpointDescription = EndpointHelper.CreateEndpointDescription(url, secPolicy, msgSecMode);
		return await CreateConnection(endpointDescription, userAuth, userName, password, sessionName, sessionTimeout);
	}


	/// <summary>Establishes the connection to an OPC UA server and creates a session using an EndpointDescription.</summary>
	/// <param name="endpointDescription">The EndpointDescription of the server's endpoint</param>
	/// <param name="userAuth">Autheticate anonymous or with username and password</param>
	/// <param name="userName">The user name</param>
	/// <param name="password">The password</param>
	/// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
	public async Task<IOpcClient> CreateConnection(EndpointDescription endpointDescription, bool userAuth, string userName = default, string password = default, string sessionName = "MySession", uint sessionTimeout = 60000)
	{
		//todo: null check
		var applicationConfig = await ApplicationConfigurationFactory.Create();
		IOpcClient client = new OpcUaClient(applicationConfig);
		UserIdentity userIdentity = new();
		try
		{
			//Create EndPoint configuration
			EndpointConfiguration endpointConfiguration = EndpointConfiguration.Create(applicationConfig);

			//Connect to server and get endpoints
			ConfiguredEndpoint endpoint = new(null, endpointDescription, endpointConfiguration);

			//Create user identity
			if (userAuth)
			{
				userIdentity = new UserIdentity(userName, password);
			}

			//Update certificate store before connection attempt
			await applicationConfig.CertificateValidator.Update(applicationConfig);

			//Create and connect session

			client.Session = await Session.Create(
				applicationConfig,
				endpoint,
				true,
				sessionName,
				sessionTimeout,
				userIdentity,
				null
				);


		}
		catch (Exception e)
		{
			StringBuilder exceptionMessageBuilder = new($"Connection [{endpointDescription.EndpointUrl}] failed. ");
			if (userAuth) exceptionMessageBuilder.Append($"User [{userName}].");
			throw new ConnectionFailedException(exceptionMessageBuilder.ToString(), e.InnerException);
		}
		return client;
	}

	/// <summary>Closes an existing session and disconnects from the server.</summary>
	/// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
	public void Disconnect(IOpcClient client)
	{
		//todo: null check
		// Close the session.
		try
		{
			client?.Session?.Close(10000);
			if (client?.Session?.Disposed == false) client?.Session?.Dispose();
		}
		catch (Exception)
		{
			if (client?.Session?.Disposed == false) client?.Session?.Dispose();
			//handle Exception here
			throw;
		}
	}

	#endregion Connect/Disconnect

}
