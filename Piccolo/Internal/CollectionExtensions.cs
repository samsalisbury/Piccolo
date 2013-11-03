using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Piccolo.Internal
{
	internal static class CollectionExtensions
	{
		internal static Dictionary<string, string> ToDictionary(this NameValueCollection source)
		{
			return source.AllKeys.ToDictionary(key => key.ToLower(), key => source[key]);
		}

		internal static TValue GetValue<TValue>(this IDictionary<string, TValue> source, string key)
		{
			if (source.ContainsKey(key) == false)
				return default(TValue);

			return source[key];
		}
	}
}