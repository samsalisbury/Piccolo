using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Piccolo.Request.HandlerInvokers
{
	public abstract class BaseRequestHandlerInvoker : IRequestHandlerInvoker
	{
		public abstract string MethodName { get; }

		public HttpResponseMessage Execute(IRequestHandler requestHandler, Dictionary<string, string> routeParameters)
		{
			var handlerType = requestHandler.GetType();
			var handlerMethod = handlerType.GetMethod(MethodName);

			foreach (var routeParameter in routeParameters)
			{
				var property = handlerType.GetProperties().Single(x => x.Name.Equals(routeParameter.Key, StringComparison.InvariantCultureIgnoreCase));
				// TODO: Refactor and expose via config
				if (property.PropertyType == typeof(Int32))
				{
					property.SetValue(requestHandler, Int32.Parse(routeParameter.Value), null);
				}
				else
				{
					property.SetValue(requestHandler, routeParameter.Value, null);
				}
			}

			// TODO: implement post binding
			var parameters = handlerMethod.GetParameters();
			var arguments = new object[parameters.Length];

			var result = handlerMethod.Invoke(requestHandler, arguments);

			var messageProperty = result.GetType().GetProperty("Message");
			return messageProperty.GetValue(result, null) as HttpResponseMessage;
		}
	}
}