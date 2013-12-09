using System;
using System.Collections.Generic;
using System.Linq;
using Piccolo.Internal;

namespace Piccolo.Routing
{
	internal static class RequestHandlerDescriptor
	{
		private static readonly Dictionary<string, string> _requestHandlerVerbMap = new Dictionary<string, string>
		{
			{typeof(IGet<>).Name, "get"},
			{typeof(IPost<,>).Name, "post"},
			{typeof(IPut<,>).Name, "put"},
			{typeof(IPatch<,>).Name, "patch"},
			{typeof(IDelete<,>).Name, "delete"}
		};

		public static IList<RouteAttribute> GetRouteAttributes(Type requestHandler)
		{
			return requestHandler.GetAttributes<RouteAttribute>();
		}

		public static string GetVerb(Type requestHandler)
		{
			var interfaces = requestHandler.GetInterfaces();
			var match = _requestHandlerVerbMap.SingleOrDefault(pair => interfaces.Any(x => x.Name == pair.Key));

			if (match.Key == null)
				throw new InvalidOperationException(ExceptionMessageBuilder.BuildInvalidRequestHandlerImplementationMessage(requestHandler));

			return match.Value;
		}
	}
}