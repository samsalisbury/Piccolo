using System.Net.Http;
using Piccolo.Routing;

namespace Piccolo.UnitTests
{
	[Route("/data/resources/{id}")]
	public class TestRequestHandler : IGet<string>
	{
		public HttpResponseMessage<string> Get()
		{
			return new HttpResponseMessage<string>(new HttpResponseMessage());
		}

		public int Id { get; set; }
	}
}