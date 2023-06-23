using FluentResults;
using MediatR;
using Opc.Ua;
using Opc.Ua.Client;
using OpcUa.Domain.Errors;

namespace OpcUa.Client.Applications.Client.Subscibtions.Commands.SubscribeNodeList;

public class SubscribeNodeListCommandHandler : IRequestHandler<SubscribeNodeListCommand, Result>
{
    public async Task<Result> Handle(SubscribeNodeListCommand request, CancellationToken cancellationToken)
    {
        Dictionary<string, NodeId> nodeIds = request.Node.ToDictionary(s => $"Id_{request.Client.Id}/{s.Name}",
            s => s.Node.NodeId, StringComparer.OrdinalIgnoreCase);

        foreach (var nodeKeyPair in nodeIds)
        {
            if (request.Client.Subscription.MonitoredItems.Any(s => s.DisplayName == nodeKeyPair.Key) ||
                request.Client.SubscribedNodes.Keys.Any(s => s == nodeKeyPair.Key))
                nodeIds.Remove(nodeKeyPair.Key);
        }
        var monitoredItems = nodeIds.Select(s =>
             new MonitoredItem()
             {
                 //Set the name of the item for assigning items and values later on; make sure item names differ
                 DisplayName = s.Key,
                 //Set the NodeId of the item
                 StartNodeId = s.Value,
                 //Set the attribute Id (value here)
                 AttributeId = Attributes.Value,
                 //Set reporting mode
                 MonitoringMode = request.MonitoringMode,
                 //Set the sampling interval (1 = fastest possible)
                 SamplingInterval = request.Interval,
                 //Set the queue size
                 QueueSize = 1,
                 //Discard the oldest item after new one has been received
                 DiscardOldest = true,

             }
        );
        foreach (var item in monitoredItems)
        {
            //Create a monitored item
            //Define event handler for this item and then add to monitoredItem
            item.Notification += request.Client.ItemChangedNotification;
        }

        try
        {
            //Add the item to the subscription
            request.Client.Subscription.AddItems(monitoredItems);
            //Apply changes to the subscription
            await request.Client.Subscription.ApplyChangesAsync();
            foreach (var item in monitoredItems)
            {
                request.Client.SubscribedNodes.Add(item.DisplayName, request.Node.First(s => s.Node.NodeId == item.StartNodeId));
            }
        }
        catch (Exception e)
        {
            return Result.Fail(DomainErrors.Opc.Client.Subscriptions.SubscribeNodeError.CausedBy(e));
        }
        return Result.Ok();
    }

}