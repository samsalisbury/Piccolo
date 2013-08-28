using Piccolo.Events;

namespace Piccolo.Tasks.EventHandlers.Logging
{
	public class LogRequestProcessedEvent : IHandle<RequestProcessedEvent>
	{
		public void Handle(RequestProcessedEvent args)
		{
			args.Context.Http.Trace.Write("RequestProcessedEvent received.");
		}
	}
}