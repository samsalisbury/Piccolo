using System;

namespace Piccolo.Validation
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class ValidateWithAttribute : Attribute
	{
		public ValidateWithAttribute(Type validatorType)
		{
			ValidatorType = validatorType;
		}

		public Type ValidatorType { get; private set; }
	}
}