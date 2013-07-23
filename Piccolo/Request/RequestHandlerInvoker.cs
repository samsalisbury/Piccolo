using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Newtonsoft.Json;
using Piccolo.Request.ParameterBinders;

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

		public HttpResponseMessage Execute(IRequestHandler requestHandler, string verb, Dictionary<string, string> routeParameters, Dictionary<string, string> queryParameters, string payload)
		{
			var handlerType = requestHandler.GetType();
			var handlerMethod = handlerType.GetMethod(verb, bindingFlags);
			var properties = handlerType.GetProperties();

			BindRouteParameters(requestHandler, routeParameters, properties);
			BindQueryParameters(requestHandler, queryParameters, properties);

			var parameters = handlerMethod.GetParameters();
			var arguments = new object[parameters.Length];
			if (parameters.Length == 1)
			{
				var parameterType = parameters.First().ParameterType;
				arguments[0] = JsonConvert.DeserializeObject(payload, parameterType); // TODO: move to config
			}

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

		private void BindQueryParameters(IRequestHandler requestHandler, Dictionary<string, string> queryParameters, IEnumerable<PropertyInfo> properties)
		{
			var optionalProperties = properties.Where(x => x.GetCustomAttributes(typeof(OptionalAttribute), true).Any());

			foreach (var property in optionalProperties)
			{
				var queryParameter = queryParameters.SingleOrDefault(x => x.Key.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase));
				if (queryParameter.Equals(default(KeyValuePair<string, string>)))
					continue;

				var binder = _routeParameterBinders.Single(x => x.Key == property.PropertyType).Value;
				binder.BindRouteParameter(requestHandler, property, queryParameter.Value);
			}
		}
	}
}