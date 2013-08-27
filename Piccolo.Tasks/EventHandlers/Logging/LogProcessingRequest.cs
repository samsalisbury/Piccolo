using System.Diagnostics;
using Piccolo.Events;

namespace Piccolo.Tasks.EventHandlers.Logging
{
	public class LogProcessingRequest : IHandle<RequestProcessingEvent>
	{
		public void Handle(RequestProcessingEvent @event)
		{
			Trace.WriteLine("RequestProcessingEvent received.");
		}
	}
}