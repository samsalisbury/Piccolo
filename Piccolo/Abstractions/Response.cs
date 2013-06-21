using System.Net;
using System.Net.Http;

namespace Piccolo.Abstractions
{
	public class Response
	{
		public class Success
		{
			public static HttpResponseMessage<dynamic> Ok()
			{
				var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);
				return new HttpResponseMessage<dynamic>(responseMessage);
			}

			public static HttpResponseMessage<TOutput> Ok<TOutput>(TOutput content)
			{
				var responseMessage = new HttpResponseMessage(HttpStatusCode.OK) {Content = new StringContent(content.ToString())};
				return new HttpResponseMessage<TOutput>(responseMessage);
			}

			public static HttpResponseMessage<dynamic> Created()
			{
				var responseMessage = new HttpResponseMessage(HttpStatusCode.Created);
				return new HttpResponseMessage<dynamic>(responseMessage);
			}
		}
	}
}