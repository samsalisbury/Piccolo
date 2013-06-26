﻿namespace Piccolo.Samples.Resources
{
	[Route("/resources")]
	public class DeleteResource : IDelete
	{
		public HttpResponseMessage<dynamic> Delete()
		{
			return Response.Success.NoContent();
		}
	}
}