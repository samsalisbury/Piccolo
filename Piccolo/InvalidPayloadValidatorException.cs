using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Piccolo
{
	[Serializable]
	[ExcludeFromCodeCoverage]
	public class InvalidPayloadValidatorException : Exception
	{
		public InvalidPayloadValidatorException()
		{
		}

		internal InvalidPayloadValidatorException(string message) : base(message)
		{
		}

		internal InvalidPayloadValidatorException(string message, Exception inner) : base(message, inner)
		{
		}

		private InvalidPayloadValidatorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}