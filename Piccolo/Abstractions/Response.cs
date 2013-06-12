using System.Net;
using System.Net.Http;

namespace Piccolo.Abstractions
{
	public class Response
	{
		public class Success
		{
			public static HttpResponseMessage<TOutput> Ok<TOutput>(TOutput content)
			{
				var responseMessage = new HttpResponseMessage(HttpStatusCode.OK) {Content = new StringContent(content.ToString())};
				return new HttpResponseMessage<TOutput>(responseMessage);
			}
		}
	}
}