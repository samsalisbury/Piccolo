namespace Piccolo.Samples.Resources
{
	[Route("/resources/{id}")]
	public class DeleteResource : IDelete
	{
		public HttpResponseMessage<dynamic> Delete()
		{
			return Response.Success.NoContent();
		}

		public int Id { get; set; }
	}
}