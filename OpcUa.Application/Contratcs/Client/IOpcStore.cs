using Opc.Ua.Client;

namespace OpcUa.Application.Contratcs.Client;

public interface IOpcStore
{
	/// <summary>
	/// Provides the event for value changes of a monitored item.
	/// </summary>
	public MonitoredItemNotificationEventHandler ItemChangedNotification { get; set; }
	/// <summary>
	/// Provides the event for a monitored event item.
	/// </summary>
	public NotificationEventHandler ItemEventNotification { get; set; }

	public void Notification_MonitoredEventItem(ISession session, NotificationEventArgs e);
}
