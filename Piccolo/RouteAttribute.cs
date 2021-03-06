using System;

namespace Piccolo
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public class RouteAttribute : Attribute
	{
		public RouteAttribute(string template)
		{
			Template = template;
		}

		public string Template { get; private set; }
	}
}