namespace Piccolo
{
	public interface IPost<in TInput> : IRequestHandler
	{
		HttpResponseMessage<dynamic> Post(TInput parameters);
	}
}