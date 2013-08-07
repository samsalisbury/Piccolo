using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Piccolo.Request.ParameterBinders;
using Piccolo.Routing;

namespace Piccolo.Configuration
{
	public class HttpHandlerConfiguration
	{
		public IRequestHandlerFactory RequestHandlerFactory { get; set; }
		public ReadOnlyCollection<Type> RequestHandlers { get; set; }
		public Dictionary<Type, IParameterBinder> RouteParameterBinders { get; set; }
		public IRequestRouter Router { get; set; }
		public Func<object, string> JsonEncoder { get; set; }
		public Func<Type, string, object> JsonDecoder { get; set; }
	}
}