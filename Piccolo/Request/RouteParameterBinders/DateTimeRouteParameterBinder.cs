using System;
using System.Reflection;

namespace Piccolo.Request.RouteParameterBinders
{
	public class DateTimeRouteParameterBinder : IRouteParameterBinder
	{
		public void BindRouteParameter(IRequestHandler requestHandler, PropertyInfo property, string rawValue)
		{
			property.SetValue(requestHandler, DateTime.Parse(rawValue), null);
		}
	}
}