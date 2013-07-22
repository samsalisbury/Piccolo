namespace Piccolo.Samples.Resources
{
	[Route("/resources")]
	public class UpdateResource : IPut<UpdateResource.CreateResourceParams>
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