using System;
using System.Collections.Generic;

namespace Piccolo.Configuration
{
	public class EventHandlers
	{
		public EventHandlers()
		{
			RequestProcessing = new List<Type>();
			RequestProcessed = new List<Type>();
			RequestFaulted = new List<Type>();
		}

		public IList<Type> RequestProcessing { get; internal set; }
		public IList<Type> RequestProcessed { get; internal set; }
		public IList<Type> RequestFaulted { get; internal set; }
	}
}