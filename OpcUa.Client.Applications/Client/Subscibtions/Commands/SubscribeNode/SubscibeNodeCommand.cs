using FluentResults;
using MediatR;
using Opc.Ua;
using Opc.Ua.Client;
using OpcUa.Domain;
using OpcUa.Domain.Contracts.Client;
using OpcUa.Domain.Errors;
using System.Text;

namespace OpcUa.Client.Applications.Client.Subscriptions.Commands.SubscribeNode;

public record SubscribeNodeCommand(IOpcClient Client, OpcNode Node, MonitoringMode MonitoringMode, int Interval, string CustomName = "") : IRequest<Result>;

public class SubscribeNodeCommandHandler : IRequestHandler<SubscribeNodeCommand, Result>
{
    public async Task<Result> Handle(SubscribeNodeCommand request, CancellationToken cancellationToken)
    {
        StringBuilder stringBuilder = new($"Id_{request.Client.Id}/");
        stringBuilder.Append(request.CustomName == "" ? request.Node.Name : request.CustomName);
        string monitoredItemName = stringBuilder.ToString();
        if (request.Client.Subscription.MonitoredItems.Any(s => s.DisplayName == monitoredItemName) ||
            request.Client.SubscribedNodes.Keys.Any(s => s == monitoredItemName))
            return Result.Fail(new SubscriptionAlreadyExist(request.Client, monitoredItemName));
        //Create a monitored item
        MonitoredItem monitoredItem = new()
        {
            //Set the name of the item for assigning items and values later on; make sure item names differ
            DisplayName = monitoredItemName,
            //Set the NodeId of the item
            StartNodeId = request.Node.Node.NodeId,
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

        };
        //Define event handler for this item and then add to monitoredItem
        monitoredItem.Notification += request.Client.ItemChangedNotification;
        request.Client.SubscribedNodes.Add(monitoredItemName, request.Node);
        try
        {
            //Add the item to the subscription
            request.Client.Subscription.AddItem(monitoredItem);
            //Apply changes to the subscription
            await request.Client.Subscription.ApplyChangesAsync();
        }
        catch (Exception e)
        {
            request.Client.SubscribedNodes.Remove(monitoredItemName);
            return Result.Fail(new SubscribingNodeError(request.Client, request.Node.Node.NodeId, e));
        }
        return Result.Ok();
    }

}

public record SubscribeNodeListCommand(IOpcClient Client, IEnumerable<OpcNode> Node, MonitoringMode MonitoringMode, int Interval) : IRequest<Result>;

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
            return Result.Fail(new SubscribingNodeError(request.Client, request.Node.First().Node.NodeId, e));
        }
        return Result.Ok();
    }

}