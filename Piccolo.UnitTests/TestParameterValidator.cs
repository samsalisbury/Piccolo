using System.Net;
using Piccolo.Validation;

namespace Piccolo.UnitTests
{
	public class TestParameterValidator : IParameterValidator<int>
	{
		public ValidationResult Validate(int value)
		{
			if (value < 0)
				return new ValidationResult(HttpStatusCode.BadRequest, "invalid age");

			return ValidationResult.Valid;
		}
	}
}