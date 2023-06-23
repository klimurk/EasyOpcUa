using FluentResults;
using MediatR;
using Opc.Ua;
using OpcUa.Domain;
using OpcUa.Domain.Contracts.Client;

namespace OpcUa.Client.Applications.Client.Subscriptions.Commands.SubscribeNode;

public record SubscribeNodeCommand(IOpcClient Client, OpcNode Node, MonitoringMode MonitoringMode, int Interval, string CustomName = "") : IRequest<Result>;
