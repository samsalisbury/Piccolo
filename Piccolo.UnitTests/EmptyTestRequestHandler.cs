using System.Net.Http;
using Piccolo.Routing;

namespace Piccolo.UnitTests
{
	[Route("/EmptyTestRequestHandler")]
	public class EmptyTestRequestHandler : IGet<string>
	{
		public HttpResponseMessage<string> Get()
		{
			return new HttpResponseMessage<string>(new HttpResponseMessage());
		}
	}
}