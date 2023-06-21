//using FluentValidation;

//namespace OpcUa.Client.Applications.Client.Nodes.Queries.GetNode;

//public class GetNodeQueryValidator : AbstractValidator<GetNodeQuery>
//{
//    public GetNodeQueryValidator()
//    {
//        RuleFor(query =>
//            query.Client).NotNull().Must(s => !s.Session.Disposed);
//        RuleFor(query =>
//            query.NodeId).NotEmpty();
//    }
//}