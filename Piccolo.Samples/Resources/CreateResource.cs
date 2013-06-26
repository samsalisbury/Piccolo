namespace Piccolo.Samples.Resources
{
	[Route("/resources")]
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