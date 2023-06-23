using FluentResults;
using MediatR;
using Opc.Ua;
using OpcUa.Domain.Errors;

namespace OpcUa.Client.Applications.Client.Nodes.Commands.WriteNodeValue
{
    public class WriteNodeValueCommandHandler : IRequestHandler<WriteNodeValueCommand, Result>
    {
        public async Task<Result> Handle(WriteNodeValueCommand request, CancellationToken cancellationToken)
        {
            //Overwrite the dataValue with a new constructor using read dataType
            DataValue dataValue = new();
            try
            {
                dataValue = request.Client.Session.ReadValue(request.node.NodeId);

            }
            catch (Exception e)
            {
                return Result.Fail(DomainErrors.Opc.Client.Reading.ReadNodeError.CausedBy(e));
            }
            Type nodeType = dataValue.Value.GetType();
            Type tryWriteType = request.Value.GetType();
            Variant variant;
            if (nodeType != tryWriteType)
            {
                try
                {
                    variant = new(Convert.ChangeType(request.Value, nodeType));
                }
                catch(Exception ex)
                {
                    return Result.Fail( DomainErrors.Opc.Client.Writing.WrongWriteValueTypeError
                        .CausedBy(ex)
                        .WithMetadata("Expected type", nodeType)
                        .WithMetadata("Attempt write type", tryWriteType));
                }
            }
            else
            {
                variant = new(request.Value);
            }
            //Create a WriteValue using the NodeId, dataValue and attributeType
            WriteValue valueToWrite = new()
            {
                Value = new(variant),
                NodeId = request.node.NodeId,
                AttributeId = Attributes.Value
            };

            WriteValueCollection valuesToWrite = new()
            {
                valueToWrite
            };
            StatusCodeCollection result = new();
            DiagnosticInfoCollection diagnostics = new();
            try
            {
                request.Client.Session.Write(requestHeader: null, valuesToWrite, out result, out diagnostics);
            }
            catch (Exception ex)
            {
                return Result.Fail(DomainErrors.Opc.Client.Writing.WriteNodeValueError.CausedBy(ex));
            }
            var errors = result.Where(s => s != StatusCodes.Good).Select(s => DomainErrors.Opc.Client.Writing.NotGoodResultError.WithMetadata(nameof(StatusCode), s));
            return errors.Any() ? Result.Fail(errors) : Result.Ok();
        }
    }
}
