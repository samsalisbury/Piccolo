namespace Piccolo.Samples.Resources
{
	[Route("/resources/{id}")]
	public class UpdateResource : IPut<UpdateResource.Parameters>
	{
		public int Id { get; set; }

		public HttpResponseMessage<dynamic> Put(Parameters parameters)
		{
			return Response.Success.NoContent();
		}

		public class Parameters
		{
		}
	}
}