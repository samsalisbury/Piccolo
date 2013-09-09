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

		public MalformedParameterException(string message) : base(message)
		{
		}

		public MalformedParameterException(string message, Exception inner) : base(message, inner)
		{
		}

		protected MalformedParameterException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}