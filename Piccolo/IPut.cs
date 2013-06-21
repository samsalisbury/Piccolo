namespace Piccolo
{
	public interface IPut<in TInput> : IRequestHandler
	{
		HttpResponseMessage<dynamic> Put(TInput parameters);
	}
}