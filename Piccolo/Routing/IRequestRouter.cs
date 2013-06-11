using System;

namespace Piccolo.Routing
{
	public interface IRequestRouter
	{
		Type GetRequestHandlerForUri(Uri requestUri);
	}
}