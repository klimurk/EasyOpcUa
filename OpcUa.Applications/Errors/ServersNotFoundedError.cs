using FluentResults;
using System.Text;

namespace OpcUa.Applications.Errors;

public class ServersNotFoundedError : Error
{
	public ServersNotFoundedError(string uri, Exception ex)
		: base(BuildError(uri, ex)) =>
		Metadata.Add(nameof(ErrorCodes.ServersNotFoundCode), ErrorCodes.ServersNotFoundCode);

	private static string BuildError(string uri, Exception ex)
	{
		StringBuilder sb = new($"Servers not found on uri {uri}");
		if (ex != null) sb.Append($"Exception {ex}");
		return sb.ToString();
	}
}
