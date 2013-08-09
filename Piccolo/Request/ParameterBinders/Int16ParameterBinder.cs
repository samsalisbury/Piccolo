using System;
using System.Reflection;

namespace Piccolo.Request.ParameterBinders
{
	public class Int16ParameterBinder : IParameterBinder
	{
		public void BindParameter(IRequestHandler requestHandler, PropertyInfo property, string rawValue)
		{
			property.SetValue(requestHandler, Int16.Parse(rawValue), null);
		}
	}
}