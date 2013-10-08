using System.Collections.Generic;
using System.Linq;
using System.Net;
using Piccolo.Validation;

namespace Piccolo.UnitTests
{
	public class TestRequestValidator : IRequestValidator
	{
		public ValidationResult Validate(IDictionary<string, string> routeParameters, string payload)
		{
			if (routeParameters.Any() == false)
				return new ValidationResult(HttpStatusCode.BadRequest, "id missing");

			return ValidationResult.Valid;
		}
	}
}