using Opc.Ua;
using Opc.Ua.Client;

namespace Woodnailer.Application.Opc.Client;

public class OpcStore
{
	/// <summary>
	/// Provides the event for value changes of a monitored item.
	/// </summary>
	public MonitoredItemNotificationEventHandler ItemChangedNotification { get; set; }

	/// <summary>
	/// Provides the event for a monitored event item.
	/// </summary>
	public NotificationEventHandler ItemEventNotification { get; set; }

	public async void Notification_MonitoredEventItem(ISession session, NotificationEventArgs e)
	{
		NotificationMessage message = e.NotificationMessage;

		// Check for keep alive.
		if (message.NotificationData.Count == 0) return;

		ItemEventNotification(session, e);
	}
}
