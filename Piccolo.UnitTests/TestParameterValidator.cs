using Piccolo.Validation;

namespace Piccolo.UnitTests
{
	public class TestParameterValidator : IParameterValidator<int>
	{
		public ValidationResult Validate(int value)
		{
			if (value < 0)
				return new ValidationResult("invalid age");

			return ValidationResult.Valid;
		}
	}
}