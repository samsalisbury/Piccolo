using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using Newtonsoft.Json;
using Piccolo.Internal;
using Piccolo.Validation;

namespace Piccolo.Request
{
	public class RequestHandlerInvoker : IRequestHandlerInvoker
	{
		private readonly Func<Type, string, object> _jsonDeserialiser;
		private readonly IDictionary<Type, Func<string, object>> _parsers;
		
		public RequestHandlerInvoker(Func<Type, string, object> jsonDeserialiser, IDictionary<Type, Func<string, object>> parsers)
		{
			_jsonDeserialiser = jsonDeserialiser;
			_parsers = parsers;
		}

		public HttpResponseMessage Execute(IRequestHandler requestHandler, string verb, IDictionary<string, string> routeParameters, IDictionary<string, string> queryParameters, IDictionary<string, object> contextualParameters, string rawPayload, object payloadValidator)
		{
			BindRouteParameters(requestHandler, routeParameters);
			BindContextualParameters(requestHandler, contextualParameters);

			var queryParameterValidationResult = BindQueryParameters(requestHandler, queryParameters);
			if (queryParameterValidationResult.IsValid == false)
				return BadRequest(queryParameterValidationResult.ErrorMessage);

			var payload = DeserialisePayload(requestHandler, verb, rawPayload);
			var payloadValidationResult = ValidatePayload(payloadValidator, payload);
			if (payloadValidationResult.IsValid == false)
				return BadRequest(payloadValidationResult.ErrorMessage);

			return GetResponseMessage(requestHandler.InvokeMethod<object>(verb, payload));
		}

		private HttpResponseMessage BadRequest(string message)
		{
			return new HttpResponseMessage(HttpStatusCode.BadRequest) {Content = new ObjectContent(new {error = message})};
		}

		private void BindRouteParameters(IRequestHandler requestHandler, IEnumerable<KeyValuePair<string, string>> routeParameters)
		{
			var requestHandlerType = requestHandler.GetType();

			foreach (var parameter in routeParameters)
			{
				var property = requestHandlerType.GetProperty(parameter.Key, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
				var parser = _parsers.SingleOrDefault(x => x.Key == property.PropertyType).Value;
				if (parser == null)
					throw new InvalidOperationException(ExceptionMessageBuilder.BuildUnsupportedParameterTypeMessage(property));

				try
				{
					var value = parser(parameter.Value);
					property.SetValue(requestHandler, value, null);
				}
				catch (FormatException)
				{
					throw new RouteParameterDatatypeMismatchException(ExceptionMessageBuilder.BuildInvalidParameterAssignmentMessage(property, parameter.Value));
				}
			}
		}

		private static void BindContextualParameters(IRequestHandler requestHandler, IDictionary<string, object> contextualParameters)
		{
			var properties = requestHandler.GetType().GetPropertiesDecoratedWith<ContextualAttribute>();

			foreach (var property in properties)
			{
				var parameter = contextualParameters.SingleOrDefault(x => x.Key.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase));
				property.SetValue(requestHandler, parameter.Value, null);
			}
		}

		private ValidationResult BindQueryParameters(IRequestHandler requestHandler, IDictionary<string, string> queryParameters)
		{
			var properties = requestHandler.GetType().GetPropertiesDecoratedWith<OptionalAttribute>();

			foreach (var property in properties)
			{
				var parameter = queryParameters.GetValue(property.Name.ToLower());
				if (parameter == null)
					continue;

				var parser = _parsers.SingleOrDefault(x => x.Key == property.PropertyType).Value;
				if (parser == null)
					throw new InvalidOperationException(ExceptionMessageBuilder.BuildUnsupportedParameterTypeMessage(property));

				var validatorAttribute = property.GetCustomAttributes(typeof(ValidateWithAttribute), true).SingleOrDefault();
				var parameterValidatorType = validatorAttribute != null ? ((ValidateWithAttribute)validatorAttribute).ValidatorType : null;

				try
				{
					var value = parser(parameter);
					if (parameterValidatorType != null)
					{
						var parameterValidator = Activator.CreateInstance(parameterValidatorType);
						var validationResult = parameterValidator.InvokeMethod<ValidationResult>("validate", new[] {value});
						if (validationResult.IsValid == false)
							return validationResult;
					}

					property.SetValue(requestHandler, value, null);
				}
				catch (FormatException)
				{
					throw new MalformedParameterException(ExceptionMessageBuilder.BuildInvalidParameterAssignmentMessage(property, parameter));
				}
			}

			return ValidationResult.Valid;
		}

		private object[] DeserialisePayload(IRequestHandler requestHandler, string verb, string payload)
		{
			var type = requestHandler.GetType().GetMethodParameterType(verb);
			if (type == null)
				return new object[0];

			if (string.IsNullOrWhiteSpace(payload))
				throw new MissingPayloadException();

			try
			{
				return new[] {_jsonDeserialiser(type, payload)};
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

		private static ValidationResult ValidatePayload(object payloadValidator, object[] payload)
		{
			if (payloadValidator == null)
				return ValidationResult.Valid;

			return payloadValidator.InvokeMethod<ValidationResult>("validate", payload);
		}

		private static HttpResponseMessage GetResponseMessage(object result)
		{
			return result.GetPropertyValue<HttpResponseMessage>("message");
		}
	}

	public interface IRequestHandlerInvoker
	{
		HttpResponseMessage Execute(IRequestHandler requestHandler, string verb, IDictionary<string, string> routeParameters, IDictionary<string, string> queryParameters, IDictionary<string, object> contextualParameters, string rawPayload, object payloadValidator);
	}
}