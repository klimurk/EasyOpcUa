﻿using FluentValidation;

namespace OpcUa.Client.Applications.Client.NodeIds.Queries.GetValue;

public class ReadValueQueryValidator : AbstractValidator<ReadValueQuery>
{
    public ReadValueQueryValidator()
    {
        RuleFor(query => query.client).NotNull().Must(s => !s.Session.Disposed);
        RuleFor(query => query.nodeId).NotEmpty();
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