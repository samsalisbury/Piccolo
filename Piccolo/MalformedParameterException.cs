using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Piccolo
{
	[Serializable]
	[ExcludeFromCodeCoverage]
	public class MalformedParameterException : Exception
	{
		public MalformedParameterException()
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