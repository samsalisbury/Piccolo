using System.Net;
using Piccolo.Validation;

namespace Piccolo.Tasks.Validators
{
	public class PageSizeValidator : IParameterValidator<int>
	{
		public ValidationResult Validate(int value)
		{
			if (value < 1)
				return new ValidationResult(HttpStatusCode.BadRequest, "pageSize must be greater than 0.");

			return ValidationResult.Valid;
		}
	}
}