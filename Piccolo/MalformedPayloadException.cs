using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Piccolo
{
	[Serializable]
	[ExcludeFromCodeCoverage]
	public class MalformedPayloadException : Exception
	{
		public MalformedPayloadException()
		{
		}

		public MalformedPayloadException(string message) : base(message)
		{
		}

		public MalformedPayloadException(string message, Exception inner) : base(message, inner)
		{
		}

		protected MalformedPayloadException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}