using System.Net.Http;

namespace Piccolo
{
	public interface IGet<out TOutput> : IRequestHandler
	{
		HttpResponseMessage Get();
	}
}