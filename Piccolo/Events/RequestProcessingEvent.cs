namespace Piccolo.Events
{
	public class RequestProcessingEvent : IEvent
	{
		public bool StopRequestProcessing { get; set; }
		public PiccoloContext Context { get; set; }
		public bool StopEventProcessing { get; set; }
	}
}