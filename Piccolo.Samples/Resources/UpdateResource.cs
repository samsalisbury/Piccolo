namespace Piccolo.Samples.Resources
{
	[Route("/resources/{id}")]
	public class UpdateResource : IPut<UpdateResource.CreateResourceParams>
	{
		public HttpResponseMessage<dynamic> Put(CreateResourceParams parameters)
		{
			return Response.Success.NoContent();
		}

		public int Id { get; set; }

		public class CreateResourceParams
		{
		}
	}
}