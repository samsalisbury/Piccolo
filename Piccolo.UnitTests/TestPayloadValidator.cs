using Piccolo.UnitTests.Request;
using Piccolo.Validation;

namespace Piccolo.UnitTests
{
	public class TestPayloadValidator : IPayloadValidator<RequestHandlerInvokerTests.TestResourceWithPayload.Parameters>
	{
		public ValidationResult Validate(RequestHandlerInvokerTests.TestResourceWithPayload.Parameters payload)
		{
			if (payload.A == string.Empty)
				return new ValidationResult("invalid");

			return ValidationResult.Valid;
		}
	}
}