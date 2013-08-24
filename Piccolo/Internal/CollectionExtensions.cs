using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Piccolo.Internal
{
	internal static class CollectionExtensions
	{
		public static Dictionary<string, string> ToDictionary(this NameValueCollection value)
		{
			return value.AllKeys.ToDictionary(key => key, key => value[key]);
		}
	}
}