using System.Net.Http;

namespace Piccolo
{
	public interface IGet<TOutput> : IRequestHandler
	{
		HttpResponseMessage<TOutput> Get();
	}
}