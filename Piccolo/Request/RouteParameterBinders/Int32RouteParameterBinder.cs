using System;
using System.Reflection;

namespace Piccolo.Request.RouteParameterBinders
{
	public class Int32RouteParameterBinder : IRouteParameterBinder
	{
		public void BindRouteParameter(IRequestHandler requestHandler, PropertyInfo property, string rawValue)
		{
			property.SetValue(requestHandler, Int32.Parse(rawValue), null);
		}
	}
}