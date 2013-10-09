using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Newtonsoft.Json;
using Piccolo.Internal;
using Piccolo.Request.ParameterBinders;
using Piccolo.Validation;

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

		public HttpResponseMessage Execute(IRequestHandler requestHandler, string verb, IDictionary<string, string> routeParameters, IDictionary<string, string> queryParameters, IDictionary<string, object> contextualParameters, string payload, object payloadValidator)
		{
			var handlerType = requestHandler.GetType();
			var handlerMethod = handlerType.GetMethod(verb, MethodLookupFlags);
			var properties = handlerType.GetProperties();
			var postParameter = DeserialisePostParameter(payload, handlerMethod);

			if (payloadValidator != null)
			{
				var validatorType = payloadValidator.GetType();
				var validationMethod = validatorType.GetMethod("validate", MethodLookupFlags);

				var validationResult = (ValidationResult)validationMethod.Invoke(payloadValidator, postParameter);
				if (validationResult.IsValid == false)
					return validationResult.ErrorResponse;
			}

			BindRouteParameters(requestHandler, routeParameters, properties);
			BindQueryParameters(requestHandler, queryParameters, properties);
			BindContextualParameters(requestHandler, contextualParameters, properties);

			var result = handlerMethod.Invoke(requestHandler, postParameter);
			return GetResponseMessage(result);
		}

		private void BindRouteParameters(IRequestHandler requestHandler, IEnumerable<KeyValuePair<string, string>> routeParameters, PropertyInfo[] properties)
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
					throw new RouteParameterDatatypeMismatchException(ExceptionMessageBuilder.BuildInvalidParameterAssignmentMessage(property, routeParameter.Value));
				}
			}
		}

		private void BindQueryParameters(IRequestHandler requestHandler, IDictionary<string, string> queryParameters, IEnumerable<PropertyInfo> properties)
		{
			var optionalProperties = properties.Where(x => x.GetCustomAttributes(typeof(OptionalAttribute), true).Any());

			foreach (var property in optionalProperties)
			{
				var queryParameter = queryParameters.SingleOrDefault(x => x.Key.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase));
				if (queryParameter.Equals(default(KeyValuePair<string, string>)))
					continue;

				var binder = _routeParameterBinders.SingleOrDefault(x => x.Key == property.PropertyType).Value;
				if (binder == null)
					throw new InvalidOperationException(ExceptionMessageBuilder.BuildUnsupportedParameterTypeMessage(property));

				try
				{
					binder.BindParameter(requestHandler, property, queryParameter.Value);
				}
				catch (FormatException)
				{
					throw new MalformedParameterException(ExceptionMessageBuilder.BuildInvalidParameterAssignmentMessage(property, queryParameter.Value));
				}
			}
		}

		private static void BindContextualParameters(IRequestHandler requestHandler, IDictionary<string, object> contextualParameters, IEnumerable<PropertyInfo> properties)
		{
			var contextualProperties = properties.Where(x => x.GetCustomAttributes(typeof(ContextualAttribute), true).Any());

			foreach (var property in contextualProperties)
			{
				var contextualParameter = contextualParameters.SingleOrDefault(x => x.Key.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase));
				property.SetValue(requestHandler, contextualParameter.Value, null);
			}
		}

		private object[] DeserialisePostParameter(string payload, MethodInfo handlerMethod)
		{
			var parameters = handlerMethod.GetParameters();
			if (parameters.Length == 0)
				return new object[0];

			if (string.IsNullOrWhiteSpace(payload))
				throw new MissingPayloadException();

			try
			{
				var parameterType = parameters.First().ParameterType;
				return new[] {_jsonDecoder(parameterType, payload)};
			}
			catch (JsonSerializationException jsex)
			{
				throw new MalformedPayloadException("Failed to deserialise request payload.", jsex);
			}
			catch (JsonReaderException jrex)
			{
				throw new MalformedPayloadException("Failed to deserialise request payload.", jrex);
			}
		}

		private static HttpResponseMessage GetResponseMessage(object result)
		{
			var messageProperty = result.GetType().GetProperty("Message");
			return messageProperty.GetValue(result, null) as HttpResponseMessage;
		}
	}
}