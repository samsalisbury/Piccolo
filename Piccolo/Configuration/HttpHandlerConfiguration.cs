using System;
using System.Collections.Generic;
using Piccolo.Request.ParameterBinders;
using Piccolo.Routing;

namespace Piccolo.Configuration
{
	public class HttpHandlerConfiguration
	{
		public IRequestHandlerFactory RequestHandlerFactory { get; set; }
		public IList<Type> RequestHandlers { get; internal set; }
		public Dictionary<Type, IParameterBinder> RouteParameterBinders { get; internal set; }
		public IRequestRouter Router { get; internal set; }
		public Func<object, string> JsonSerialiser { get; set; }
		public Func<Type, string, object> JsonDeserialiser { get; set; }
	}
}