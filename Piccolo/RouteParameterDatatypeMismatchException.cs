using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Piccolo
{
	[Serializable]
	[ExcludeFromCodeCoverage]
	public class RouteParameterDatatypeMismatchException : Exception
	{
		public RouteParameterDatatypeMismatchException()
		{
		}

		public RouteParameterDatatypeMismatchException(string message) : base(message)
		{
		}

		public RouteParameterDatatypeMismatchException(string message, Exception inner) : base(message, inner)
		{
		}

		protected RouteParameterDatatypeMismatchException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}