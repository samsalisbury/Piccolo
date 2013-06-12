using System.Net.Http;

namespace Piccolo
{
	public class HttpResponseMessage<TOutput>
	{
		public HttpResponseMessage(HttpResponseMessage responseMessage)
		{
			Message = responseMessage;
		}

		public HttpResponseMessage Message { get; private set; }
	}
}