using System;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Piccolo.Internal;

namespace Piccolo.Request
{
	public class PayloadDeserialiser : IPayloadDeserialiser
	{
		private readonly Func<Type, string, object> _jsonDeserialiser;

		public PayloadDeserialiser(Func<Type, string, object> jsonDeserialiser)
		{
			_jsonDeserialiser = jsonDeserialiser;
		}

		public object DeserialisePayload(IRequestHandler requestHandler, string verb, string payload)
		{
			var type = requestHandler.GetType().GetMethodParameterType(verb);
			if (type == null)
				return null;

			if (string.IsNullOrWhiteSpace(payload))
				throw new MissingPayloadException();

			try
			{
				return _jsonDeserialiser(type, payload);
			}
			catch (JsonSerializationException jsex)
			{
				throw new MalformedPayloadException("Failed to deserialise request payload.", jsex);
			}
			catch (JsonReaderException jrex)
			{
				throw new MalformedPayloadException("Failed to deserialise request payload.", jrex);
			}
			catch (FormatException formatException)
			{
				throw new MalformedPayloadException("Failed to deserialise request payload.", formatException);
			}
		}
	}

	public interface IPayloadDeserialiser
	{
		object DeserialisePayload(IRequestHandler requestHandler, string verb, string payload);
	}
}