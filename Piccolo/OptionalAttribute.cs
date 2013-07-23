using System;

namespace Piccolo
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class OptionalAttribute : Attribute
	{
	}
}