using System;
using System.Reflection;

namespace Piccolo.Request.ParameterBinders
{
	public class Int16ParameterBinder : IRouteParameterBinder
	{
		public void BindRouteParameter(IRequestHandler requestHandler, PropertyInfo property, string rawValue)
		{
			property.SetValue(requestHandler, Int16.Parse(rawValue), null);
		}
	}
}