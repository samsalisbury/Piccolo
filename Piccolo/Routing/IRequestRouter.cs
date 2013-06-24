using System;

namespace Piccolo.Routing
{
	public interface IRequestRouter
	{
		Type FindRequestHandler(string verb, Uri requestUri);
	}
}