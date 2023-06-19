//using Opc.Ua.Client;
//using Opc.Ua;
//using OpcUa.Helpers;
//using System.Text;
//using OpcUa.Domain;
//using OpcUa.Domain.Contracts.Client;
//using OpcUa.Persistance.Exceptions.Connection;
//using OpcUa.Client.Applications.Contracts.Client;

//namespace OpcUa.Persistance.Client;

//public class OpcConnector : IOpcConnector
//{
//	#region Connect/Disconnect

//	/// <summary>Establishes the connection to an OPC UA server and creates a session using a server url.</summary>
//	/// <param name="url">The Url of the endpoint as string.</param>
//	/// <param name="secPolicy">The security policy to use</param>
//	/// <param name="msgSecMode">The message security mode to use</param>
//	/// <param name="userAuth">Autheticate anonymous or with username and password</param>
//	/// <param name="userName">The user name</param>
//	/// <param name="password">The password</param>
//	/// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>


//	/// <summary>Closes an existing session and disconnects from the server.</summary>
//	/// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
//	public void Disconnect(IOpcClient client)
//	{
//		//todo: null check
//		// Close the session.
//		try
//		{
//			client?.Session?.Close(10000);
//			if (client?.Session?.Disposed == false) client?.Session?.Dispose();
//		}
//		catch (Exception)
//		{
//			if (client?.Session?.Disposed == false) client?.Session?.Dispose();
//			//handle Exception here
//			throw;
//		}
//	}

//	#endregion Connect/Disconnect

//}
