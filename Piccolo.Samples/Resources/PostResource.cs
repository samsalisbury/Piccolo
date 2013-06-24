namespace Piccolo.Samples.Resources
{
	[Route("/resource")]
	public class CreateResource : IPost<CreateResource.CreateResourceParams>
	{
		public HttpResponseMessage<dynamic> Post(CreateResourceParams parameters)
		{
			return Response.Success.Created();
		}

		public class CreateResourceParams
		{
		}
	}
}