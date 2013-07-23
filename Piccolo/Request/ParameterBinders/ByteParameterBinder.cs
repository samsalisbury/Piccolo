using System;
using System.Reflection;

namespace Piccolo.Request.ParameterBinders
{
	public class ByteParameterBinder : IRouteParameterBinder
	{
		public void BindRouteParameter(IRequestHandler requestHandler, PropertyInfo property, string rawValue)
		{
			property.SetValue(requestHandler, Byte.Parse(rawValue), null);
		}
	}
}