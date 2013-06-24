using System.Net.Http;

namespace Piccolo.Request.HandlerInvokers
{
	public interface IRequestHandlerInvoker
	{
		HttpResponseMessage Execute(IRequestHandler requestHandler, IRequestContextWrapper requestContext);
	}
}