namespace Piccolo.Samples.Resources
{
	[Route("/resource")]
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