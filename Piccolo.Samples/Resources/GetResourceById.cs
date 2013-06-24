namespace Piccolo.Samples.Resources
{
	[Route("/resource")]
	public class GetResourceById : IGet<string>
	{
		public HttpResponseMessage<string> Get()
		{
			return Response.Success.Ok("Adam West!");
		}
	}
}