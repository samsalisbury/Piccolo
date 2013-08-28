using Piccolo.Events;

namespace Piccolo.Tasks.EventHandlers.Logging
{
	public class LogRequestProcessingEvent : IHandle<RequestProcessingEvent>
	{
		public void Handle(RequestProcessingEvent args)
		{
			args.Context.Http.Trace.Write("RequestProcessingEvent received.");
		}
	}
}