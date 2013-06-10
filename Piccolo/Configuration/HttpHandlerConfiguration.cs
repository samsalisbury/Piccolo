using System;
using System.Collections.ObjectModel;
using Piccolo.IoC;

namespace Piccolo.Configuration
{
	public class HttpHandlerConfiguration
	{
		internal HttpHandlerConfiguration()
		{
		}

		public IRequestHandlerFactory RequestHandlerFactory { get; set; }
		public ReadOnlyCollection<Type> RequestHandlers { get; internal set; }
	}
}