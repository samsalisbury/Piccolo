namespace Piccolo
{
	public interface IPut<in TInput, TOutput> : IRequestHandler
	{
		HttpResponseMessage<TOutput> Put(TInput task);
	}
}