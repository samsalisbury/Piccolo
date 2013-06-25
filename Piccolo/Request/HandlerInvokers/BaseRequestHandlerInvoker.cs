using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Piccolo.Request.RouteParameterBinders;

namespace Piccolo.Request.HandlerInvokers
{
	public abstract class BaseRequestHandlerInvoker : IRequestHandlerInvoker
	{
		private readonly Dictionary<Type, IRouteParameterBinder> _routeParameterBinders;

		protected BaseRequestHandlerInvoker(Dictionary<Type, IRouteParameterBinder> routeParameterBinders)
		{
			_routeParameterBinders = routeParameterBinders;
		}

		public abstract string MethodName { get; }

		public HttpResponseMessage Execute(IRequestHandler requestHandler, Dictionary<string, string> routeParameters)
		{
			var handlerType = requestHandler.GetType();
			var handlerMethod = handlerType.GetMethod(MethodName);

			BindRouteParameters(requestHandler, routeParameters, handlerType.GetProperties());

			// TODO: implement post binding
			var parameters = handlerMethod.GetParameters();
			var arguments = new object[parameters.Length];

			var result = handlerMethod.Invoke(requestHandler, arguments);

			var messageProperty = result.GetType().GetProperty("Message");
			return messageProperty.GetValue(result, null) as HttpResponseMessage;
		}

		private void BindRouteParameters(IRequestHandler requestHandler, Dictionary<string, string> routeParameters, PropertyInfo[] properties)
		{
			foreach (var routeParameter in routeParameters)
			{
				var property = properties.Single(x => x.Name.Equals(routeParameter.Key, StringComparison.InvariantCultureIgnoreCase));
				var binder = _routeParameterBinders.Single(x => x.Key == property.PropertyType).Value;
				binder.BindRouteParameter(requestHandler, property, routeParameter.Value);
			}
		}
	}
}