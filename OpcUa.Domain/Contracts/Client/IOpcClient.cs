using Opc.Ua.Client;
using Opc.Ua;
using System.Collections.ObjectModel;
using FluentResults;

namespace OpcUa.Domain.Contracts.Client;

public interface IOpcClient
{
    string Id { get; }
    ApplicationConfiguration AppConfiguration { get; }
    ISession Session { get; }
    string Name { get; set; }
    public Subscription Subscription { get; }
    IDictionary<string, OpcNode> SubscribedNodes{ get; }
    public MonitoredItemNotificationEventHandler ItemChangedNotification { get; set; }
    public Result CreateSubscription(int publishingInterval);
    public Result RemoveSubscription(bool isSilent = true);
}