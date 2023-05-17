using Opc.Ua.Client;
using Opc.Ua;
using OpcUa.Domain;
using OpcUa.Domain.Contracts.Client;
using OpcUa.Application.Contratcs.Client;

namespace Woodnailer.Application.Opc.Client;

public class OpcSubscriber
{
	private readonly IOpcStore opcStore;

	public OpcSubscriber(IOpcStore opcStore)
	{
		this.opcStore = opcStore;
	}
	#region Subscription

	/// <summary>Ads a monitored event item to an existing subscription</summary>
	/// <param name="subscription">The subscription</param>
	/// <param name="nodeIdString">The node Id as string</param>
	/// <param name="itemName">The name of the item to add</param>
	/// <param name="samplingInterval">The sampling interval</param>
	/// <param name="filter">The event filter</param>
	/// <returns>The added item</returns>
	/// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
	public MonitoredItem AddEventMonitoredItem(IOpcClient client, Subscription subscription, string nodeIdString, string itemName, int samplingInterval, EventFilter filter)
	{
		//Create a monitored item
		MonitoredItem monitoredItem = new(subscription.DefaultItem)
		{
			//Set the name of the item for assigning items and values later on; make sure item names differ
			DisplayName = itemName,
			//Set the NodeId of the item
			StartNodeId = nodeIdString,
			//Set the attribute Id (value here)
			AttributeId = Attributes.EventNotifier,
			//Set reporting mode
			MonitoringMode = MonitoringMode.Reporting,
			//Set the sampling interval (1 = fastest possible)
			SamplingInterval = samplingInterval,
			//Set the queue size
			QueueSize = 1,
			//Discard the oldest item after new one has been received
			DiscardOldest = true,
			//Set the filter for the event item
			Filter = filter
		};

		//Define event handler for this item and then add to monitoredItem
		client.NotificationEventNotification += new NotificationEventHandler(opcStore.Notification_MonitoredEventItem);

		try
		{
			//Add the item to the subscription
			subscription.AddItem(monitoredItem);
			//Apply changes to the subscription
			subscription.ApplyChanges();
			return monitoredItem;
		}
		catch (Exception)
		{
			//handle Exception here
			throw;
		}
	}

	/// <summary>Ads a monitored item to an existing subscription</summary>
	/// <param name="subscription">The subscription</param>
	/// <param name="nodeIdString">The node Id as string</param>
	/// <param name="itemName">The name of the item to add</param>
	/// <param name="samplingInterval">The sampling interval</param>
	/// <returns>The added item</returns>
	/// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
	public MonitoredItem AddMonitoredItem(Subscription subscription, string nodeIdString, string itemName, int samplingInterval)
	{
		//Create a monitored item
		MonitoredItem monitoredItem = new()
		{
			//Set the name of the item for assigning items and values later on; make sure item names differ
			DisplayName = itemName,
			//Set the NodeId of the item
			StartNodeId = nodeIdString,
			//Set the attribute Id (value here)
			AttributeId = Attributes.Value,
			//Set reporting mode
			MonitoringMode = MonitoringMode.Reporting,
			//Set the sampling interval (1 = fastest possible)
			SamplingInterval = samplingInterval,
			//Set the queue size
			QueueSize = 1,
			//Discard the oldest item after new one has been received
			DiscardOldest = true
		};
		//Define event handler for this item and then add to monitoredItem
		monitoredItem.Notification += new MonitoredItemNotificationEventHandler(Notification_MonitoredItem);
		try
		{
			//Add the item to the subscription
			subscription.AddItem(monitoredItem);
			//Apply changes to the subscription
			subscription.ApplyChanges();
			errorConnect = false;
			return monitoredItem;
		}
		catch (Exception e)
		{
			//handle Exception here
			errorConnect = true; return null;
			throw e;
		}
	}

	/// <summary>Removs a monitored item from an existing subscription</summary>
	/// <param name="subscription">The subscription</param>
	/// <param name="monitoredItem">The item</param>
	/// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
	public static MonitoredItem RemoveMonitoredItem(Subscription subscription, MonitoredItem monitoredItem)
	{
		try
		{
			//Add the item to the subscription
			subscription.RemoveItem(monitoredItem);
			//Apply changes to the subscription
			subscription.ApplyChanges();
			return null;
		}
		catch (Exception)
		{
			//handle Exception here
			throw;
		}
	}

	/// <summary>Removes an existing Subscription</summary>
	/// <param name="subscription">The subscription</param>
	/// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
	public static void RemoveSubscription(Subscription subscription)
	{
		try
		{
			//Delete the subscription and all items submitted
			subscription.Delete(true);
		}
		catch (Exception)
		{
			//handle Exception here
			throw;
		}
	}

	/// <summary>Creats a Subscription object to a server</summary>
	/// <param name="publishingInterval">The publishing interval</param>
	/// <returns>Subscription</returns>
	/// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
	public Subscription Subscribe(int publishingInterval)
	{
		//Create a Subscription object
		Subscription subscription = new(mSession.DefaultSubscription)
		{
			//Enable publishing
			PublishingEnabled = true,
			//Set the publishing interval
			PublishingInterval = publishingInterval
		};
		//Add the subscription to the session
		mSession.AddSubscription(subscription);
		try
		{
			//Create/Activate the subscription
			subscription.Create();
			return subscription;
		}
		catch (Exception)
		{
			//handle Exception here
			throw;
		}
	}

	#endregion Subscription

}
