using FluentValidation;

namespace OpcUa.Client.Applications.Client.Nodes.Queries.GetValue;

//public class ReadNodeValueQueryValidator : AbstractValidator<ReadNodeValueQuery>
//{
//    public ReadNodeValueQueryValidator()
//    {
//        RuleFor(query => query.client).NotNull().Must(s => !s.Session.Disposed);
//        RuleFor(query => query.nodes).NotEmpty();
       
//    }
//}