using System.Collections.Generic;
using System.Net.Http;

namespace Piccolo.Request.HandlerInvokers
{
	public interface IRequestHandlerInvoker
	{
		HttpResponseMessage Execute(IRequestHandler requestHandler, Dictionary<string, string> routeParameters);
	}
}