using System;

namespace Piccolo.Request
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class OptionalAttribute : Attribute
	{
	}
}