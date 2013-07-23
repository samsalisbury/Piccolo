namespace Piccolo.Samples.Resources
{
	[Route("/resources/{id}")]
	public class DeleteResource : IDelete
	{
		public int Id { get; set; }

		public HttpResponseMessage<dynamic> Delete()
		{
			return Response.Success.NoContent();
		}
	}
}