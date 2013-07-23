using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Piccolo
{
	[ExcludeFromCodeCoverage]
	internal static class CollectionExtensions
	{
		public static Dictionary<string, string> ToDictionary(this NameValueCollection value)
		{
			return value.AllKeys.ToDictionary(key => key, key => value[key]);
		}
	}
}