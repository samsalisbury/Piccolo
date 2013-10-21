namespace Piccolo.Events
{
	public class RequestProcessedEvent : IEvent
	{
		public string Payload { get; set; }
		public PiccoloContext Context { get; set; }
		public bool StopEventProcessing { get; set; }
	}
}