namespace Piccolo
{
	public interface IPost<TInput> : IRequestHandler
	{
		HttpResponseMessage<TInput> Post(TInput parameters);
	}
}