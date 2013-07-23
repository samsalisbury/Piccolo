using System;
using System.Reflection;

namespace Piccolo.Request.ParameterBinders
{
	public class BooleanParameterBinder : IRouteParameterBinder
	{
		public void BindRouteParameter(IRequestHandler requestHandler, PropertyInfo property, string rawValue)
		{
			property.SetValue(requestHandler, Boolean.Parse(rawValue), null);
		}
	}
}