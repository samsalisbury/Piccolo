namespace Piccolo.Validation
{
	public interface IParameterValidator<in T>
	{
		ValidationResult Validate(T value);
	}
}