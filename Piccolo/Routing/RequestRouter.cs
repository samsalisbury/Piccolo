using System;
using Piccolo.Configuration;

namespace Piccolo.Routing
{
	public class RequestRouter : IRequestRouter
	{
		private readonly RouteHandlerLookup _routeHandlerLookup;

		public RequestRouter(HttpHandlerConfiguration configuration)
		{
			_routeHandlerLookup = new RouteHandlerLookup(configuration.RequestHandlers);
		}

		public RouteHandlerLookupResult FindRequestHandler(string verb, Uri requestUri)
		{
			return _routeHandlerLookup.FindRequestHandler(verb, requestUri.AbsolutePath);
		}
	}
}