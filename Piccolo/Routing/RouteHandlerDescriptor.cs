using System;
using System.Collections.Generic;
using System.Linq;

namespace Piccolo.Routing
{
	public static class RouteHandlerDescriptor
	{
		private static readonly string[] _reservedPropertyNames = new[] {"output"};

		public static List<RouteAttribute> GetRouteAttributes(Type requestHandler)
		{
			return requestHandler.GetCustomAttributes(typeof(RouteAttribute), true).Cast<RouteAttribute>().ToList();
		}

		public static Dictionary<string, Type> GetRequestHandlerProperties(Type requestHandler)
		{
			if (requestHandler == null)
				return new Dictionary<string, Type>();

			var allProperties = requestHandler.GetProperties();
			var allInputProperties = allProperties.Where(x => _reservedPropertyNames.Contains(x.Name.ToLower()) == false);

			return allInputProperties.ToDictionary(x => x.Name.ToLower(), x => x.PropertyType);
		}

		public static string GetVerb(Type requestHandler)
		{
			return "get";
		}
	}
}