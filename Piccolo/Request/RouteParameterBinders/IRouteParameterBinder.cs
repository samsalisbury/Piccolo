using System.Reflection;

namespace Piccolo.Request.RouteParameterBinders
{
	public interface IRouteParameterBinder
	{
		void BindRouteParameter(IRequestHandler requestHandler, PropertyInfo property, string rawValue);
	}
}