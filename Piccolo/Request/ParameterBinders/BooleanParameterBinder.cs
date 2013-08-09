using System;
using System.Reflection;

namespace Piccolo.Request.ParameterBinders
{
	public class BooleanParameterBinder : IParameterBinder
	{
		public void BindParameter(IRequestHandler requestHandler, PropertyInfo property, string rawValue)
		{
			property.SetValue(requestHandler, Boolean.Parse(rawValue), null);
		}
	}
}