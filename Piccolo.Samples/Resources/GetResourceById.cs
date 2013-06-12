using Piccolo.Abstractions;
using Piccolo.Routing;

namespace Piccolo.Samples.Resources
{
	[Route("/")]
	public class GetResourceById : IGet<string>
	{
		public HttpResponseMessage<string> Get()
		{
			return Response.Success.Ok("Adam West!");
		}
	}
}