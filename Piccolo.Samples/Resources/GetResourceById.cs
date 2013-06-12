using System.Net.Http;
using Piccolo.Abstractions;
using Piccolo.Routing;

namespace Piccolo.Samples.Resources
{
	[Route("/")]
	public class GetResourceById : IGet<string>
	{
		public HttpResponseMessage Get()
		{
			return Response.Success.Ok("Adam West!");
		}
	}
}