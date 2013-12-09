using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Piccolo.Configuration
{
	public class PiccoloConfiguration
	{
		public IObjectFactory ObjectFactory { get; set; }
		public ReadOnlyCollection<Type> RequestHandlers { get; internal set; }
		public IDictionary<Type, Func<string, object>> Parsers { get; internal set; }
		public Func<object, string> JsonSerialiser { get; set; }
		public Func<Type, string, object> JsonDeserialiser { get; set; }
		public EventHandlers EventHandlers { get; internal set; }
	}
}