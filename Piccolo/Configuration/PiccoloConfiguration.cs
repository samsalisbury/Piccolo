using System;
using System.Collections.Generic;
using Piccolo.Request.ParameterBinders;

namespace Piccolo.Configuration
{
	public class PiccoloConfiguration
	{
		public IObjectFactory ObjectFactory { get; set; }
		public IList<Type> RequestHandlers { get; internal set; }
		public IDictionary<Type, IParameterBinder> ParameterBinders { get; internal set; }
		public Func<object, string> JsonSerialiser { get; set; }
		public Func<Type, string, object> JsonDeserialiser { get; set; }
		public EventHandlers EventHandlers { get; internal set; }
	}
}