using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Piccolo.Request.HandlerInvokers;
using Piccolo.Request.RouteParameterBinders;
using Piccolo.Routing;

namespace Piccolo.Configuration
{
	public class HttpHandlerConfiguration
	{
		public IRequestHandlerFactory RequestHandlerFactory { get; set; }
		public ReadOnlyCollection<Type> RequestHandlers { get; set; }
		public Dictionary<Type, IRouteParameterBinder> RouteParameterBinders { get; set; }
		public Dictionary<string, IRequestHandlerInvoker> RequestHandlerInvokers { get; set; }
		public IRequestRouter Router { get; set; }
	}
}