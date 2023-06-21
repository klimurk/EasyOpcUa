using FluentResults;

namespace OpcUa.Domain.Errors;

public static class DomainErrors
{
    public static class Client
    {
        public static class Certificates
        {
            public static readonly Error ValidationError = new("Certificates. Validation error.");
        }
    }
    public static class Endpoint
    {

    }

    public static class Server
    {
        public static readonly Error NotFoundError = new ("Opc Client. Cannot find server");
        //public static readonly Error ConnectionFailed = new ()
    }

}
