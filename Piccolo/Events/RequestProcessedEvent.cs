namespace Piccolo.Events
{
	public class RequestProcessedEvent : IEvent
	{
		public PiccoloContext Context { get; set; }
		public bool StopProcessing { get; set; }
	}
}