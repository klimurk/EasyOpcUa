using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcUa.Application.Helpers
{
	public static class OpcTypeValueHelper
	{
		public static bool IsValueTypeValid(object value) =>
				value is bool || value is bool[]
			|| value is byte || value is byte[]
			|| value is short || value is short[]
			|| value is ushort || value is ushort[]
			|| value is int || value is int[]
			|| value is uint || value is uint[]
			|| value is long || value is long[]
			|| value is ulong || value is ulong[]
			|| value is double || value is double[]
			|| value is float || value is float[]
			|| value is string || value is string[]
			|| value is char || value is char[];
	}
}
