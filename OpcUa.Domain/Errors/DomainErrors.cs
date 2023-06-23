using FluentResults;

namespace OpcUa.Domain.Errors;

public static class DomainErrors
{
    public static class Opc
    {
        public static class Client
        {
            public static class Certificates
            {
                public static Error ValidationError { get => new Error("Certificates. Validation error."); }
            }
            public static class Browsing
            {
                public static Error BrowsingRootError { get => new Error("Cannot read root."); }
                public static Error BrowsingReferenceError { get => new Error("Cannot read inner references."); }
                public static Error BrowsingNodeError { get => new Error("Cannot read node inner references."); }
            }
            public static class Endpoints
            {
                public static Error NotFoundError { get => new Error("Endpoints not found."); }

            }
            public static class Reading
            {
                public static Error ReadNodeError { get => new Error("Cannot read node value."); }
                public static Error NotGoodResultError { get => new Error("Read node value with errors."); }
            }
            public static class Server
            {
                public static Error NotFoundError { get => new("Opc server not found"); }
                public static Error CannotConnectError { get => new("Cannot connect to server"); }

                //public static readonly Error ConnectionFailed = new ()
            }
            public static class Subscriptions
            {

                public static Error CreateSubscriptionError { get => new Error("Error while create subscription on client."); }
                public static Error RemoveSubscriptionError { get => new Error("Cannot remove subscription subscription on client."); }
                public static Error SubscriptionWithNameExistError { get => new Error("Subscription with name already exist."); }
                public static Error SubscribeNodeError { get => new Error("Cannot subscribe node."); }

                public static Error NotGoodResultError { get => new Error("Read node value with errors."); }
            }
            public static class Writing
            {
                public static Error WriteNodeValueError { get => new Error("Error while write node value."); }
                public static Error NotGoodResultError { get => new Error("Write node value with errors."); }
                public static Error WrongWriteValueTypeError { get => new("Type mismatch while writing node value."); }

            }
        }

        public static class Server
        {

        }
    }
    public static class Validation
    {
        public static Error ValidationFailedError { get => new Error("Validation failed"); }
    }
   
}
