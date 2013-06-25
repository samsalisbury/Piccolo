using System.Net.Http;

namespace Piccolo.Request.HandlerInvokers
{
	public class GetRequestHandlerInvoker : BaseRequestHandlerInvoker
	{
		public override string MethodName
		{
			get { return "Get"; }
		}
	}

	public abstract class BaseRequestHandlerInvoker : IRequestHandlerInvoker
	{
		public abstract string MethodName { get; }

		public HttpResponseMessage Execute(IRequestHandler requestHandler)
		{
			var handlerType = requestHandler.GetType();
			var handlerMethod = handlerType.GetMethod(MethodName);

			var parameters = handlerMethod.GetParameters();
			var arguments = new object[parameters.Length];
			var result = handlerMethod.Invoke(requestHandler, arguments);

			var messageProperty = result.GetType().GetProperty("Message");
			return messageProperty.GetValue(result, null) as HttpResponseMessage;
		}
	}
}