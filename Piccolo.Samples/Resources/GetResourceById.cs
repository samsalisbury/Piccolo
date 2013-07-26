using Piccolo.Samples.Models;

namespace Piccolo.Samples.Resources
{
	[Route("/resources/{id}")]
	public class GetResourceById : IGet<Resource>
	{
		public int Id { get; set; }

		[Optional]
		public int Page { get; set; }

		public HttpResponseMessage<Resource> Get()
		{
			var resource = new Resource
			{
				Id = Id,
				Page = Page
			};
			return Response.Success.Ok(resource);
		}
	}
}