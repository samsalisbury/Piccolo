using System;
using System.Collections.Generic;
using System.Linq;

namespace Piccolo.Routing
{
	public static class RouteHandlerDescriptor
	{
		private static readonly Dictionary<string, string> _requestHandlerVerbMap = new Dictionary<string, string>
			{
				{typeof(IGet<>).Name, "get"},
				{typeof(IPut<>).Name, "put"},
				{typeof(IPost<>).Name, "post"}
			};

		public static List<RouteAttribute> GetRouteAttributes(Type requestHandler)
		{
			return requestHandler.GetCustomAttributes(typeof(RouteAttribute), true).Cast<RouteAttribute>().ToList();
		}

		public static Dictionary<string, Type> GetRequestHandlerProperties(Type requestHandler)
		{
			if (requestHandler == null)
				return new Dictionary<string, Type>();

			return requestHandler.GetProperties().ToDictionary(x => x.Name.ToLower(), x => x.PropertyType);
		}

		public static string GetVerb(Type requestHandler)
		{
			var interfaces = requestHandler.GetInterfaces();
			var match = _requestHandlerVerbMap.SingleOrDefault(pair => interfaces.Any(x => x.Name == pair.Key));

			if (match.Key == null)
				throw new InvalidOperationException(string.Format("Request handler [{0}] does not implement any of the following supported interfaces: IGet<>, IPut<>, IPost<>.", requestHandler.FullName));

			return match.Value;
		}
	}
}