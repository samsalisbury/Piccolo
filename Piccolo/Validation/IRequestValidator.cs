using System.Collections.Generic;

namespace Piccolo.Validation
{
	public interface IRequestValidator
	{
		ValidationResult Validate(IDictionary<string, string> routeParameters, string payload);
	}
}