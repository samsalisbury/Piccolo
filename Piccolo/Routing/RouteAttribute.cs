using System;

namespace Piccolo.Routing
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public class RouteAttribute : Attribute
	{
		public RouteAttribute(string uri)
		{
			Uri = uri;
		}

		public string Uri { get; private set; }
	}
}