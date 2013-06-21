using System;
using System.Collections.Generic;
using System.Linq;

namespace Piccolo.Routing
{
	public static class RouteHandlerDescriptor
	{
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

			if (interfaces.Any(x => x.Name == typeof(IGet<>).Name))
				return "get";
			if (interfaces.Any(x => x.Name == typeof(IPut<>).Name))
				return "put";
			if (interfaces.Any(x => x.Name == typeof(IPost<>).Name))
				return "post";
			else
				throw new InvalidOperationException(string.Format("Request handler [{0}] does not implement any of the following supported interfaces: IGet<>, IPut<>, IPost<>.", requestHandler.FullName));
		}
	}
}