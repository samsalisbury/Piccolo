using System.Reflection;

namespace Piccolo.Request.ParameterBinders
{
	public interface IParameterBinder
	{
		void BindParameter(IRequestHandler requestHandler, PropertyInfo property, string rawValue);
	}
}