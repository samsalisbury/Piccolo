namespace Piccolo.Events
{
	public interface IEvent
	{
		PiccoloContext Context { get; set; }
		bool StopEventProcessing { get; set; }
	}
}