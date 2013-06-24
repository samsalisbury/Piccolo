using System.Linq;
using System.Net.Http;

namespace Piccolo.Request.HandlerInvokers
{
	public class PutRequestHandlerInvoker : IRequestHandlerInvoker
	{
		public HttpResponseMessage Execute(IRequestHandler requestHandler)
		{
			// TODO: push properties
			// TODO: pass post params
			var method = requestHandler.GetType().GetMethod("Put");
			var parameters = Enumerable.Repeat("", method.GetParameters().Count()).ToArray();
			var result = method.Invoke(requestHandler, parameters);

			var messageProperty = result.GetType().GetProperty("Message");
			return messageProperty.GetValue(result, null) as HttpResponseMessage;
		}
	}
}