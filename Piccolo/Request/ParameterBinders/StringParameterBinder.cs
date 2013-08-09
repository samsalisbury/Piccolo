using System.Reflection;

namespace Piccolo.Request.ParameterBinders
{
	public class StringParameterBinder : IParameterBinder
	{
		public void BindParameter(IRequestHandler requestHandler, PropertyInfo property, string rawValue)
		{
			property.SetValue(requestHandler, rawValue, null);
		}
	}
}