using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using Piccolo.Routing;

namespace Piccolo.UnitTests
{
	[Route("/data/resources/{id}")]
	public class TestRequestHandler : IGet<string>
	{
		[ExcludeFromCodeCoverage]
		public HttpResponseMessage<string> Get()
		{
			return new HttpResponseMessage<string>(new HttpResponseMessage());
		}

		public int Id { get; set; }
	}
}