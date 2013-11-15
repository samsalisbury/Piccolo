using System;
using System.Net;
using System.Net.Http;

namespace Piccolo
{
	public class Response
	{
		public static HttpResponseMessage<TOutput> CreateErrorResponse<TOutput>(HttpStatusCode statusCode, string reason)
		{
			var message = new HttpResponseMessage(statusCode) {Content = new ObjectContent(new {error = reason})};
			return new HttpResponseMessage<TOutput>(message);
		}

		private static HttpResponseMessage<TOutput> CreateSuccessResponse<TOutput>(HttpStatusCode statusCode, TOutput content)
		{
			return new HttpResponseMessage<TOutput>(new HttpResponseMessage(statusCode) {Content = new ObjectContent(content)});
		}

		private static HttpResponseMessage<TOutput> CreateEmptyResponse<TOutput>(HttpStatusCode statusCode)
		{
			return new HttpResponseMessage<TOutput>(new HttpResponseMessage(statusCode));
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
				return CreateEmptyResponse<TOutput>(HttpStatusCode.NotFound);
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
				return CreateSuccessResponse(HttpStatusCode.OK, content);
			}

			public static HttpResponseMessage<TOutput> Created<TOutput>(TOutput content, Uri createdResourceUri)
			{
				var response = CreateSuccessResponse(HttpStatusCode.Created, content);
				response.Message.Headers.Location = createdResourceUri;

				return response;
			}

			public static HttpResponseMessage<TOutput> NoContent<TOutput>()
			{
				return CreateEmptyResponse<TOutput>(HttpStatusCode.NoContent);
			}
		}
	}
}