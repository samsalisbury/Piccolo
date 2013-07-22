using System;
using System.Reflection;

namespace Piccolo.Request.RouteParameterBinders
{
	public class BooleanRouteParameterBinder : IRouteParameterBinder
	{
		public void BindRouteParameter(IRequestHandler requestHandler, PropertyInfo property, string rawValue)
		{
			property.SetValue(requestHandler, Boolean.Parse(rawValue), null);
		}
	}
}