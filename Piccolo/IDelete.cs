namespace Piccolo
{
	public interface IDelete<in TInput, TOutput> : IRequestHandler
	{
		HttpResponseMessage<TOutput> Delete(TInput parameters);
	}
}