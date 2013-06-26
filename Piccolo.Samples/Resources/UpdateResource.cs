namespace Piccolo.Samples.Resources
{
	[Route("/resources")]
	public class UpdateResource : IPost<UpdateResource.UpdateResourceParams>
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