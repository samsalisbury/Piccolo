using System;
using System.Collections.Generic;

namespace Piccolo.Routing
{
	public class RouteHandlerLookupResult
	{
		public RouteHandlerLookupResult(Type requestHandlerType, Dictionary<string, string> routeParameters)
		{
			RequestHandlerType = requestHandlerType;
			RouteParameters = routeParameters;
		}

		public Type RequestHandlerType { get; private set; }
		public Dictionary<string, string> RouteParameters { get; private set; }
	}
}