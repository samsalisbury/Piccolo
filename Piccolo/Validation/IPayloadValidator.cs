namespace Piccolo.Validation
{
	public interface IPayloadValidator<in T>
	{
		ValidationResult Validate(T payload);
	}
}