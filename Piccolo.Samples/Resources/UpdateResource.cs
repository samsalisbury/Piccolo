namespace Piccolo.Samples.Resources
{
	[Route("/resources")]
	public class CreateResource : IPost<CreateResource.UpdateResourceParams>
	{
		public HttpResponseMessage<dynamic> Post(UpdateResourceParams parameters)
		{
			return Response.Success.NoContent();
		}

		public class UpdateResourceParams
		{
		}
	}
}