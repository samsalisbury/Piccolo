namespace Piccolo.Samples.Resources
{
	[Route("/resources/{id}")]
	public class GetResourceById : IGet<string>
	{
		public HttpResponseMessage<string> Get()
		{
			return Response.Success.Ok(string.Format("Adam West! ...and here is your parameter >> {0}", Id));
		}

		public int Id { get; set; }
	}
}