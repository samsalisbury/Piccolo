namespace Piccolo.Validation
{
	public interface IPayloadValidator<in TPayload>
	{
		ValidationResult Validate(TPayload payload);
	}
}