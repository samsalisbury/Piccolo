using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Piccolo.Request.RouteParameterBinders;

namespace Piccolo.Request
{
	public class RequestHandlerInvoker
	{
		private readonly Dictionary<Type, IRouteParameterBinder> _routeParameterBinders;
		private BindingFlags bindingFlags = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public;

		public RequestHandlerInvoker(Dictionary<Type, IRouteParameterBinder> routeParameterBinders)
		{
			_routeParameterBinders = routeParameterBinders;
		}

		public HttpResponseMessage Execute(IRequestHandler requestHandler, string verb, Dictionary<string, string> routeParameters)
		{
			var handlerType = requestHandler.GetType();
			var handlerMethod = handlerType.GetMethod(verb, bindingFlags);

			BindRouteParameters(requestHandler, routeParameters, handlerType.GetProperties());

			// TODO: implement post parameter binding
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