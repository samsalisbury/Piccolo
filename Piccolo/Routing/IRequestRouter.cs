using System;

namespace Piccolo.Routing
{
	public interface IRequestRouter
	{
		RouteHandlerLookupResult FindRequestHandler(string verb, Uri requestUri);
	}
}