namespace Piccolo.Samples.Resources
{
	[Route("/resources/{id}")]
	public class GetResourceById : IGet<string>
	{
		public int Id { get; set; }

		[Optional]
		public int Page { get; set; }

		public HttpResponseMessage<string> Get()
		{
			return Response.Success.Ok(string.Format("Id: {0}; Page: {1}", Id, Page));
		}
	}
}