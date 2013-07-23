namespace Piccolo.Samples.Resources
{
	[Route("/resources/{id}")]
	public class UpdateResource : IPut<UpdateResource.CreateResourceParams>
	{
		public int Id { get; set; }

		public HttpResponseMessage<dynamic> Put(CreateResourceParams parameters)
		{
			return Response.Success.NoContent();
		}

		public class CreateResourceParams
		{
		}
	}
}