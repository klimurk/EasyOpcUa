using FluentResults;
using MediatR;
using Opc.Ua;
using OpcUa.Domain;
using OpcUa.Domain.Contracts.Client;

namespace OpcUa.Client.Applications.Client.Subscibtions.Commands.SubscribeNodeList;

public record SubscribeNodeListCommand(IOpcClient Client, IEnumerable<OpcNode> Node, MonitoringMode MonitoringMode, int Interval) : IRequest<Result>;
