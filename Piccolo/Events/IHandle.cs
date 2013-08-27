namespace Piccolo.Events
{
	public interface IHandle<in TEvent>
	{
		void Handle(TEvent @event);
	}
}