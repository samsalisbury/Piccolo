using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Piccolo.Configuration
{
	public class EventHandlers
	{
		public EventHandlers(IList<Type> requestProcessingHandlers, IList<Type> requestFaultedHandlers, IList<Type> requestProcessedHandlers)
		{
			RequestProcessing = new ReadOnlyCollection<Type>(requestProcessingHandlers);
			RequestFaulted = new ReadOnlyCollection<Type>(requestFaultedHandlers);
			RequestProcessed = new ReadOnlyCollection<Type>(requestProcessedHandlers);
		}

		public ReadOnlyCollection<Type> RequestProcessing { get; internal set; }
		public ReadOnlyCollection<Type> RequestFaulted { get; internal set; }
		public ReadOnlyCollection<Type> RequestProcessed { get; internal set; }
	}
}