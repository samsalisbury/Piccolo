using System.Net;
using System.Net.Http;

namespace Piccolo.Validation
{
	public class ValidationResult
	{
		static ValidationResult()
		{
			Valid = new ValidationResult();
		}

		private ValidationResult()
		{
			IsValid = true;
		}

		public ValidationResult(HttpStatusCode statusCode, string errorMessage)
		{
			IsValid = false;
			ErrorResponse = new HttpResponseMessage(statusCode) {Content = new ObjectContent(errorMessage)};
		}

		public static ValidationResult Valid { get; private set; }
		public bool IsValid { get; private set; }
		public HttpResponseMessage ErrorResponse { get; private set; }
	}
}