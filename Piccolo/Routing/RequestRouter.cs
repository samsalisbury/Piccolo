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

		public Type GetRequestHandlerForUri(Uri requestUri)
		{
			var absolutePath = requestUri.AbsolutePath.ToLower();

			return _routeHandlerLookup.FindRequestHandlerForPath(absolutePath);
		}
	}
}