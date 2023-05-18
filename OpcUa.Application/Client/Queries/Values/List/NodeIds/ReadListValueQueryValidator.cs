﻿using FluentValidation;

namespace OpcUa.Application.Client.Queries.Values.List.NodeIds;

public class ReadListValueQueryValidator : AbstractValidator<ReadListValueQuery>
{
    public ReadListValueQueryValidator()
    {
        RuleFor(query => query.client).NotNull().Must(s => !s.Session.Disposed);
        RuleFor(query => query.nodeIdStrings).NotEmpty();
        RuleForEach(query => query.nodeIdStrings).NotEmpty();
        RuleFor(query => query.ExpectedType).Must(s =>
        s is null
        || s.IsAssignableFrom(typeof(object)) || s.IsAssignableFrom(typeof(object[]))
        || s.IsAssignableFrom(typeof(bool)) || s.IsAssignableFrom(typeof(bool[]))
        || s.IsAssignableFrom(typeof(byte)) || s.IsAssignableFrom(typeof(byte[]))
        || s.IsAssignableFrom(typeof(short)) || s.IsAssignableFrom(typeof(short[]))
        || s.IsAssignableFrom(typeof(ushort)) || s.IsAssignableFrom(typeof(ushort[]))
        || s.IsAssignableFrom(typeof(int)) || s.IsAssignableFrom(typeof(int[]))
        || s.IsAssignableFrom(typeof(uint)) || s.IsAssignableFrom(typeof(uint[]))
        || s.IsAssignableFrom(typeof(long)) || s.IsAssignableFrom(typeof(long[]))
        || s.IsAssignableFrom(typeof(ulong)) || s.IsAssignableFrom(typeof(ulong[]))
        || s.IsAssignableFrom(typeof(double)) || s.IsAssignableFrom(typeof(double[]))
        || s.IsAssignableFrom(typeof(float)) || s.IsAssignableFrom(typeof(float[]))
        || s.IsAssignableFrom(typeof(string)) || s.IsAssignableFrom(typeof(string[]))
        || s.IsAssignableFrom(typeof(char)) || s.IsAssignableFrom(typeof(char[])));
    }
}
