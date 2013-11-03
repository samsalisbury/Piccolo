using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Piccolo.Internal
{
	[Serializable]
	[ExcludeFromCodeCoverage]
	internal class RouteParameterDatatypeMismatchException : Exception
	{
		internal RouteParameterDatatypeMismatchException()
		{
		}

		internal RouteParameterDatatypeMismatchException(string message) : base(message)
		{
		}

		internal RouteParameterDatatypeMismatchException(string message, Exception inner) : base(message, inner)
		{
		}

		private RouteParameterDatatypeMismatchException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}