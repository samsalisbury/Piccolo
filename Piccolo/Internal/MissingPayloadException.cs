using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Piccolo.Internal
{
	[Serializable]
	[ExcludeFromCodeCoverage]
	internal class MissingPayloadException : Exception
	{
		internal MissingPayloadException()
		{
		}

		internal MissingPayloadException(string message) : base(message)
		{
		}

		internal MissingPayloadException(string message, Exception inner) : base(message, inner)
		{
		}

		private MissingPayloadException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}