namespace Piccolo.Samples.Resources
{
	[Route("/resource")]
	public class DeleteResource : IDelete
	{
		public HttpResponseMessage<dynamic> Delete()
		{
			return Response.Success.NoContent();
		}
	}
}