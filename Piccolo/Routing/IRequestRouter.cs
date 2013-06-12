using System;

namespace Piccolo.Routing
{
	public interface IRequestRouter
	{
		RequestRouterResult FindRequestHandler(string verb, Uri requestUri);
	}
}