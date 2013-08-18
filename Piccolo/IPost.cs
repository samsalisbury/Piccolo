namespace Piccolo
{
	public interface IPost<in TInput, TOutput> : IRequestHandler
	{
		HttpResponseMessage<TOutput> Post(TInput parameters);
	}
}