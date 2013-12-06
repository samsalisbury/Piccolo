using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Piccolo
{
	[Serializable]
	[ExcludeFromCodeCoverage]
	public class MalformedPayloadException : Exception
	{
		internal MalformedPayloadException()
		{
		}

		internal MalformedPayloadException(string message) : base(message)
		{
		}

		internal MalformedPayloadException(string message, Exception inner) : base(message, inner)
		{
		}

		internal MalformedPayloadException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}