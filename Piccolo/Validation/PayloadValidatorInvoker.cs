using System;
using System.Linq;
using Piccolo.Configuration;
using Piccolo.Internal;

namespace Piccolo.Validation
{
	public class PayloadValidatorInvoker : IPayloadValidatorInvoker
	{
		private readonly IObjectFactory _objectFactory;

		public PayloadValidatorInvoker(IObjectFactory objectFactory)
		{
			_objectFactory = objectFactory;
		}

		public string ValidatePayload(Type requestHandlerType, string verb, object payload)
		{
			var payloadValidator = GetPayloadValidator(requestHandlerType, verb);
			if (payloadValidator == null)
				return null;

			var validationResult = payloadValidator.InvokeMethod<ValidationResult>("validate", payload);
			return validationResult.IsValid ? null : validationResult.ErrorMessage;
		}

		private object GetPayloadValidator(Type requestHandlerType, string verb)
		{
			var attribute = requestHandlerType.GetAttribute<ValidateWithAttribute>();
			if (attribute == null)
				return null;

			var validatorInterfaces = attribute.ValidatorType.GetInterfaces().Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IPayloadValidator<>)).ToList();
			if (validatorInterfaces.Any() == false)
				throw new InvalidPayloadValidatorException(ExceptionMessageBuilder.BuildInvalidPayloadValidatorMessage(attribute.ValidatorType, requestHandlerType));

			if (validatorInterfaces.Any(x => x.GetGenericArguments().First() == requestHandlerType.GetMethodParameterType(verb)) == false)
				throw new InvalidPayloadValidatorException(ExceptionMessageBuilder.BuildInvalidPayloadValidatorMessage(attribute.ValidatorType, requestHandlerType));

			return _objectFactory.CreateInstance<object>(attribute.ValidatorType);
		}
	}

	public interface IPayloadValidatorInvoker
	{
		string ValidatePayload(Type requestHandlerType, string verb, object payload);
	}
}