using System;
using System.Collections.Generic;

namespace Piccolo.Routing
{
	internal static class RouteIdentifierBuiler
	{
		internal static IList<string> BuildIdentifier(string verb, string uri)
		{
			var baseHandlerIdentifier = new List<string>(new[] {verb.ToLower(), "_root_"});

			var uriFragments = uri.Split(new[] {"/"}, StringSplitOptions.RemoveEmptyEntries);
			if (uriFragments.Length == 0)
				return baseHandlerIdentifier;

			baseHandlerIdentifier.AddRange(uriFragments);
			return baseHandlerIdentifier;
		}
	}
}