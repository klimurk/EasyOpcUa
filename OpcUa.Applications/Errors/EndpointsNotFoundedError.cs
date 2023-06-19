using FluentResults;
using System.Text;

namespace OpcUa.Applications.Errors;

public class EndpointsNotFoundedError : Error
{
	public EndpointsNotFoundedError(string uri, Exception e = default) : base(BuildError(uri, e))
	{
		Metadata.Add(nameof(ErrorCodes.EndpointsNotFoundCode), ErrorCodes.EndpointsNotFoundCode);
	}
	private static string BuildError(string uri, Exception e = default)
	{
		StringBuilder sb = new($"Endpoints on {uri} not found");
		if (e != default) sb.Append($"Exception {e}");
		return sb.ToString();
	}
}