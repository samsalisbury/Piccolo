using System.Net.Http;

namespace Piccolo
{
	public interface IRequestHandlerInvoker
	{
		HttpResponseMessage Execute(IRequestHandler requestHandler);
	}
}