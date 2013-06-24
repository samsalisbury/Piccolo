using System;
using System.Collections.ObjectModel;
using Piccolo.Routing;

namespace Piccolo.Configuration
{
	public class HttpHandlerConfiguration
	{
		public IRequestHandlerFactory RequestHandlerFactory { get; set; }
		public ReadOnlyCollection<Type> RequestHandlers { get; set; }
		public IRequestRouter Router { get; set; }
	}
}