using System.Net;
using System.Net.Http;

namespace Piccolo
{
	public class Response
	{
		public static HttpResponseMessage<TOutput> CreateErrorResponse<TOutput>(HttpStatusCode statusCode, string reason)
		{
			var responseMessage = new HttpResponseMessage(statusCode) {Content = new ObjectContent(new {error = reason})};
			return new HttpResponseMessage<TOutput>(responseMessage);
		}

		public class Error
		{
			public static HttpResponseMessage<TOutput> BadRequest<TOutput>(string reason)
			{
				return CreateErrorResponse<TOutput>(HttpStatusCode.BadRequest, reason);
			}

			public static HttpResponseMessage<TOutput> Unauthorized<TOutput>(string reason)
			{
				return CreateErrorResponse<TOutput>(HttpStatusCode.Unauthorized, reason);
			}

			public static HttpResponseMessage<TOutput> NotFound<TOutput>()
			{
				return CreateErrorResponse<TOutput>(HttpStatusCode.NotFound, string.Empty);
			}

			public static HttpResponseMessage<TOutput> Gone<TOutput>(string reason)
			{
				return CreateErrorResponse<TOutput>(HttpStatusCode.Gone, reason);
			}
		}

		public class Success
		{
			public static HttpResponseMessage<TOutput> Ok<TOutput>(TOutput content)
			{
				var responseMessage = new HttpResponseMessage(HttpStatusCode.OK) {Content = new ObjectContent(content)};
				return new HttpResponseMessage<TOutput>(responseMessage);
			}

			public static HttpResponseMessage<TOutput> Created<TOutput>(TOutput content)
			{
				var responseMessage = new HttpResponseMessage(HttpStatusCode.Created) {Content = new ObjectContent(content)};
				return new HttpResponseMessage<TOutput>(responseMessage);
			}

			public static HttpResponseMessage<TOutput> NoContent<TOutput>()
			{
				var responseMessage = new HttpResponseMessage(HttpStatusCode.NoContent);
				return new HttpResponseMessage<TOutput>(responseMessage);
			}
		}
	}
}