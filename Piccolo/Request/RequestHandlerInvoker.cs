using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Piccolo.Internal;
using Piccolo.Validation;

namespace Piccolo.Request
{
	public class RequestHandlerInvoker : IRequestHandlerInvoker
	{
		private readonly IDictionary<Type, Func<string, object>> _parsers;

		public RequestHandlerInvoker(IDictionary<Type, Func<string, object>> parsers)
		{
			_parsers = parsers;
		}

		public HttpResponseMessage Execute(IRequestHandler requestHandler, string verb, IDictionary<string, string> routeParameters, IDictionary<string, string> queryParameters, IDictionary<string, object> contextualParameters, object payload)
		{
			BindRouteParameters(requestHandler, routeParameters);
			BindContextualParameters(requestHandler, contextualParameters);

			var queryParameterValidationResult = BindQueryParameters(requestHandler, queryParameters);
			if (queryParameterValidationResult.IsValid == false)
				return new HttpResponseMessage(HttpStatusCode.BadRequest) {Content = new ObjectContent(new {error = queryParameterValidationResult.ErrorMessage})};

			return GetResponseMessage(requestHandler.InvokeMethod<object>(verb, payload));
		}

		private void BindRouteParameters(IRequestHandler requestHandler, IEnumerable<KeyValuePair<string, string>> routeParameters)
		{
			var requestHandlerType = requestHandler.GetType();

			foreach (var parameter in routeParameters)
			{
				var property = requestHandlerType.FindProperty(parameter.Key);
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

				var validatorAttribute = property.GetAttribute<ValidateWithAttribute>();
				var parameterValidatorType = validatorAttribute != null ? validatorAttribute.ValidatorType : null;

				try
				{
					var value = parser(parameter);
					if (parameterValidatorType != null)
					{
						var parameterValidator = Activator.CreateInstance(parameterValidatorType);
						var validationResult = parameterValidator.InvokeMethod<ValidationResult>("validate", value);
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

		private static HttpResponseMessage GetResponseMessage(object result)
		{
			return result.GetPropertyValue<HttpResponseMessage>("message");
		}
	}

	public interface IRequestHandlerInvoker
	{
		HttpResponseMessage Execute(IRequestHandler requestHandler, string verb, IDictionary<string, string> routeParameters, IDictionary<string, string> queryParameters, IDictionary<string, object> contextualParameters, object payload);
	}
}