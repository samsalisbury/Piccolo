using System;
using System.Collections.Generic;

namespace Piccolo.Routing
{
	public class RouteHandlerLookupResult
	{
		public static readonly RouteHandlerLookupResult FailedResult = new RouteHandlerLookupResult(null, null);

		public RouteHandlerLookupResult(Type requestHandlerType, IDictionary<string, string> routeParameters)
		{
			RequestHandlerType = requestHandlerType;
			RouteParameters = routeParameters;
		}

		public Type RequestHandlerType { get; private set; }
		public IDictionary<string, string> RouteParameters { get; private set; }

		public bool IsSuccessful
		{
			get { return RequestHandlerType == null; }
		}
	}
}