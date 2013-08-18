using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Piccolo.Internal;
using Piccolo.Request.ParameterBinders;

namespace Piccolo.Request
{
	public class RequestHandlerInvoker
	{
		private const BindingFlags MethodLookupFlags = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public;
		private readonly Func<Type, string, object> _jsonDecoder;
		private readonly IDictionary<Type, IParameterBinder> _routeParameterBinders;

		public RequestHandlerInvoker(Func<Type, string, object> jsonDecoder, IDictionary<Type, IParameterBinder> routeParameterBinders)
		{
			_jsonDecoder = jsonDecoder;
			_routeParameterBinders = routeParameterBinders;
		}

		public HttpResponseMessage Execute(IRequestHandler requestHandler, string verb, Dictionary<string, string> routeParameters, Dictionary<string, string> queryParameters, string payload)
		{
			var handlerType = requestHandler.GetType();
			var handlerMethod = handlerType.GetMethod(verb, MethodLookupFlags);
			var properties = handlerType.GetProperties();

			BindRouteParameters(requestHandler, routeParameters, properties);
			BindQueryParameters(requestHandler, queryParameters, properties);
			var postParameter = DeserialisePostParameter(payload, handlerMethod);

			var result = handlerMethod.Invoke(requestHandler, postParameter);

			return GetResponseMessage(result);
		}

		private void BindRouteParameters(IRequestHandler requestHandler, Dictionary<string, string> routeParameters, PropertyInfo[] properties)
		{
			foreach (var routeParameter in routeParameters)
			{
				var property = properties.Single(x => x.Name.Equals(routeParameter.Key, StringComparison.InvariantCultureIgnoreCase));
				var binder = _routeParameterBinders.Single(x => x.Key == property.PropertyType).Value;

				try
				{
					binder.BindParameter(requestHandler, property, routeParameter.Value);
				}
				catch (FormatException)
				{
					throw new InvalidOperationException(ExceptionMessageBuilder.BuildInvalidParameterAssignmentMessage(property, routeParameter.Value));
				}
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

				var binder = _routeParameterBinders.SingleOrDefault(x => x.Key == property.PropertyType).Value;
				if (binder == null)
					throw new InvalidOperationException(ExceptionMessageBuilder.BuildUnsupportedQueryParameterTypeMessage(property));

				try
				{
					binder.BindParameter(requestHandler, property, queryParameter.Value);
				}
				catch (FormatException)
				{
					throw new InvalidOperationException(ExceptionMessageBuilder.BuildInvalidParameterAssignmentMessage(property, queryParameter.Value));
				}
			}
		}

		private object[] DeserialisePostParameter(string payload, MethodInfo handlerMethod)
		{
			var parameters = handlerMethod.GetParameters();
			var arguments = new object[parameters.Length];
			if (parameters.Length == 1)
			{
				var parameterType = parameters.First().ParameterType;
				arguments[0] = _jsonDecoder(parameterType, payload);
			}
			return arguments;
		}

		private static HttpResponseMessage GetResponseMessage(object result)
		{
			var messageProperty = result.GetType().GetProperty("Message");
			return messageProperty.GetValue(result, null) as HttpResponseMessage;
		}
	}
}