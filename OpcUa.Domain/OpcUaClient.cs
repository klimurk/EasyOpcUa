using FluentResults;
using Opc.Ua;
using Opc.Ua.Client;
using OpcUa.Domain.Contracts.Client;
using OpcUa.Domain.Errors;
using shortid;
using System.Reactive.Subjects;
using System.Security.Cryptography.X509Certificates;

namespace OpcUa.Domain;


public class OpcUaClient : IOpcClient
{
    public string Name { get; set; }
    public string Id { get; private set; } = ShortId.Generate(new(useNumbers: false, useSpecialCharacters: false, length: 8));
    public IDictionary<string, OpcNode> SubscribedNodes { get; private set; } = new Dictionary<string, OpcNode>();
    public ReplaySubject<bool> IsOnline { get; private set; } = new(1);
    public Subscription Subscription { get; private set; }
    public ApplicationConfiguration AppConfiguration { get; private set; }
    /// <summary>
    /// Provides the session being established with an OPC UA server.
    /// </summary>
    public ISession Session { get; private set; }

    public OpcUaClient(ApplicationConfiguration SessionApplicationConfig, ISession session)
    {
        AppConfiguration = SessionApplicationConfig;
        AppConfiguration.CertificateValidator.CertificateValidation += CertificateValidationNotification;
        AppConfiguration.CertificateValidator.CertificateValidation += Notification_ServerCertificate;
        Session = session;
        Session.KeepAlive += Notification_KeepAlive;
        Session.KeepAlive += KeepAliveNotification;
        //Session.Notification += Notification_MonitoredEventItem;

        ItemChangedNotification += UpdateNodeInSubList;
    }
    private void UpdateNodeInSubList(MonitoredItem monitoredItem, MonitoredItemNotificationEventArgs e)
    {
        if (e.NotificationValue is not MonitoredItemNotification notification ||
            !SubscribedNodes.TryGetValue(monitoredItem.DisplayName, out OpcNode node))
            return;

        node.Data.OnNext(notification.Value);
    }
    #region Subscribtion 
    public Result CreateSubscription(int publishingInterval)
    {
        //Create a Subscription object
        Subscription subscription = new(Session.DefaultSubscription)
        {
            //Enable publishing
            PublishingEnabled = true,
            //Set the publishing interval
            PublishingInterval = publishingInterval,

        };
        //Add the subscription to the session
        Session.AddSubscription(subscription);
        try
        {
            //Create/Activate the subscription
            subscription.Create();
            Subscription = subscription;
            return Result.Ok();
        }
        catch (Exception ex)
        {
            //handle Exception here
            return Result.Fail(new CreateSessionSubscriptionError(this, ex));
        }
    }

    public Result RemoveSubscription(bool isSilent = true)
    {
        try
        {
            //Delete the subscription and all items submitted
            Subscription.Delete(isSilent);
            return Result.Ok();
        }
        catch (Exception e)
        {
            return Result.Fail(new RemoveSubscriptionError(this, Subscription, e));
        }
    }
    #endregion

    #region Events
    /// <summary>
	/// Provides the event for value changes of a monitored item.
	/// </summary>
	public MonitoredItemNotificationEventHandler ItemChangedNotification { get; set; }

    /// <summary>
    /// Provides the event handling for server certificates.
    /// </summary>
    private CertificateValidationEventHandler CertificateValidationNotification { get; set; }
    /// <summary>
    /// Provides the event for KeepAliveNotifications.
    /// </summary>
    private KeepAliveEventHandler KeepAliveNotification { get; set; }

    /// <summary>
    /// Provides the event for a monitored event item.
    /// </summary>
    private NotificationEventHandler NotificationEventNotification { get; set; }


    private async void Notification_KeepAlive(ISession sender, KeepAliveEventArgs e)
    {
        try
        {
            if (!ServiceResult.IsGood(e.Status))
            {
                IsOnline.OnNext(false);
                Session.Reconnect();
            }
            else
            {
                IsOnline.OnNext(true);
            }
        }
        catch (Exception ex)
        {

        }
    }
    private async void Notification_ServerCertificate(CertificateValidator cert, CertificateValidationEventArgs e)
    {
        X509CertificateCollection certCol;
        using (X509Store store = new(StoreName.Root, StoreLocation.CurrentUser))
        {
            store.Open(OpenFlags.ReadOnly);
            certCol = store.Certificates.Find(X509FindType.FindByThumbprint, e.Certificate.Thumbprint, true);
        }
        if (certCol.Capacity > 0) e.Accept = true;
    }

    private void Notification_MonitoredEventItem(ISession session, NotificationEventArgs e)
    {
        NotificationMessage message = e.NotificationMessage;

        // Check for keep alive.
        if (message.NotificationData.Count == 0) return;

        NotificationEventNotification(session, e);
    }
    #endregion
}

