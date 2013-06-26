using System;
using System.Reflection;

namespace Piccolo.Request.RouteParameterBinders
{
	public class Int16RouteParameterBinder : IRouteParameterBinder
	{
		public void BindRouteParameter(IRequestHandler requestHandler, PropertyInfo property, string rawValue)
		{
			property.SetValue(requestHandler, Int16.Parse(rawValue), null);
		}
	}
}