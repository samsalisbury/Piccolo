using System.Reflection;

namespace Piccolo.Request.ParameterBinders
{
	public class StringParameterBinder : IParameterBinder
	{
		public void BindRouteParameter(IRequestHandler requestHandler, PropertyInfo property, string rawValue)
		{
			property.SetValue(requestHandler, rawValue, null);
		}
	}
}