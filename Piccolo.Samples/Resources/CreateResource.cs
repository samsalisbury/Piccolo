namespace Piccolo.Samples.Resources
{
	[Route("/resources")]
	public class CreateResource : IPost<CreateResource.Parameters>
	{
		public HttpResponseMessage<dynamic> Post(Parameters parameters)
		{
			return Response.Success.Created();
		}

		public class Parameters
		{
		}
	}
}