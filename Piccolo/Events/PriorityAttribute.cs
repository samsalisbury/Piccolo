using System;

namespace Piccolo.Events
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class PriorityAttribute : Attribute
	{
		public PriorityAttribute(byte level)
		{
			Level = level;
		}

		public byte Level { get; private set; }
	}
}