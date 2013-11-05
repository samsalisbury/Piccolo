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

		public ValidationResult(string errorMessage)
		{
			IsValid = false;
			ErrorMessage = errorMessage;
		}

		public static ValidationResult Valid { get; private set; }
		public bool IsValid { get; private set; }
		public string ErrorMessage { get; private set; }
	}
}