using Opc.Ua.Client;
using Opc.Ua;

namespace OpcUa.Domain.Contracts.Client;

public interface IOpcClient
{
	Session Session { get; set; }
	ApplicationConfiguration SessionApplicationConfig { get; }

	/// <summary>
	/// Provides the event handling for server certificates.
	/// </summary>
	public CertificateValidationEventHandler CertificateValidationNotification { get; set; }

	/// <summary>
	/// Provides the event for a monitored event item.
	/// </summary>
	public NotificationEventHandler NotificationEventNotification { get; set; }


	/// <summary>
	/// Provides the event for KeepAliveNotifications.
	/// </summary>
	public KeepAliveEventHandler KeepAliveNotification { get; set; }

}