using System.Reflection;

namespace Piccolo.Request.ParameterBinders
{
	public interface IRouteParameterBinder
	{
		void BindRouteParameter(IRequestHandler requestHandler, PropertyInfo property, string rawValue);
	}
}