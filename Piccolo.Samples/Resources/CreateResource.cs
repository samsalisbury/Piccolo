namespace Piccolo.Samples.Resources
{
	[Route("/resource")]
	public class CreateResource : IPut<CreateResource.CreateResourceParams>
	{
		public HttpResponseMessage<dynamic> Put(CreateResourceParams parameters)
		{
			return Response.Success.Created();
		}

		public class CreateResourceParams
		{
		}
	}
}