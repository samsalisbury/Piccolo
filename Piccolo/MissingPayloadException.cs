using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Piccolo
{
	[Serializable]
	[ExcludeFromCodeCoverage]
	public class MissingPayloadException : Exception
	{
		public MissingPayloadException()
		{
		}

		public MissingPayloadException(string message) : base(message)
		{
		}

		public MissingPayloadException(string message, Exception inner) : base(message, inner)
		{
		}

		protected MissingPayloadException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}