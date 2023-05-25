﻿namespace OpcUa.Application.Opc;

public record OpcMethodArguments(OpcMethodArgumentType type, string Name = "undefined", dynamic Value = null, string Datatype = "undefined")
{
}
public enum OpcMethodArgumentType
{
	Input,
	Output
}