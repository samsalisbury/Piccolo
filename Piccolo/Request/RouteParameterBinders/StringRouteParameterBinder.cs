using System.Reflection;

namespace Piccolo.Request.RouteParameterBinders
{
	public class StringRouteParameterBinder : IRouteParameterBinder
	{
		public void BindRouteParameter(IRequestHandler requestHandler, PropertyInfo property, string rawValue)
		{
			property.SetValue(requestHandler, rawValue, null);
		}
	}
}