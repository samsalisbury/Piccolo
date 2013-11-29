namespace Piccolo
{
	public interface IPatch<in TInput, TOutput> : IRequestHandler
	{
		HttpResponseMessage<TOutput> Patch(TInput parameters);
	}
}