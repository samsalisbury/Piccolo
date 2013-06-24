namespace Piccolo
{
	public interface IDelete : IRequestHandler
	{
		HttpResponseMessage<dynamic> Delete();
	}
}