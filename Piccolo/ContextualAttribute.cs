using System;

namespace Piccolo
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class ContextualAttribute : Attribute
	{
	}
}