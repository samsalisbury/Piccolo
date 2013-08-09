using System;
using System.Reflection;

namespace Piccolo.Request.ParameterBinders
{
	public class Int32ParameterBinder : IParameterBinder
	{
		public void BindParameter(IRequestHandler requestHandler, PropertyInfo property, string rawValue)
		{
			property.SetValue(requestHandler, Int32.Parse(rawValue), null);
		}
	}
}