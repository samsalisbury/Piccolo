using System.Reflection;

namespace Piccolo.Request.ParameterBinders
{
	public interface IParameterBinder
	{
		void BindRouteParameter(IRequestHandler requestHandler, PropertyInfo property, string rawValue);
	}
}