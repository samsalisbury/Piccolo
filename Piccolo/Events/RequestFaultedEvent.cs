using System;

namespace Piccolo.Events
{
	public class RequestFaultedEvent : IEvent
	{
		public Exception Exception { get; set; }
		public PiccoloContext Context { get; set; }
		public bool StopProcessing { get; set; }
	}
}