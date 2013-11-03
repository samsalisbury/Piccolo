using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Piccolo.Internal
{
	[Serializable]
	[ExcludeFromCodeCoverage]
	internal class MalformedParameterException : Exception
	{
		internal MalformedParameterException()
		{
		}

		internal MalformedParameterException(string message) : base(message)
		{
		}

		internal MalformedParameterException(string message, Exception inner) : base(message, inner)
		{
		}

		private MalformedParameterException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}