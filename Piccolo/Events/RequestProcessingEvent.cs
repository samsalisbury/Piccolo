namespace Piccolo.Events
{
	public class RequestProcessingEvent : IEvent
	{
		public PiccoloContext Context { get; set; }
		public bool StopEventProcessing { get; set; }
	}
}