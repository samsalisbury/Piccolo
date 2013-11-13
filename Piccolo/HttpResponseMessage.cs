using System;
using System.Net;
using System.Net.Http;

namespace Piccolo
{
	public class HttpResponseMessage<TOutput>
	{
		public HttpStatusCode StatusCode{get { return Message.StatusCode; }}
		public object Content { get { return Message.Content; } }
		public Uri Location{get { return Message.Headers.Location; }}
		public string ReasonPhrase{get { return Message.ReasonPhrase; }}

		public HttpResponseMessage() : this(null, null, null)
		{
		}

		public HttpResponseMessage(HttpStatusCode statusCode)
			: this(new HttpStatusCode?(statusCode), null, null)
		{
		}

		public HttpResponseMessage(HttpContent content)
			: this(null, content, null)
		{
		}

		public HttpResponseMessage(HttpStatusCode statusCode, object content)
			: this(statusCode, content, null)
		{
		}

		public HttpResponseMessage(HttpStatusCode statusCode, HttpContent content, Uri location)
			: this((HttpStatusCode?) statusCode, content, location)
		{
		}

		public HttpResponseMessage(HttpStatusCode statusCode, HttpContent content)
			: this((HttpStatusCode?)statusCode, content, null)
		{
		}

		public HttpResponseMessage(HttpStatusCode statusCode, object content, Uri location)
			: this(new HttpStatusCode?(statusCode), content == null ? null : new ObjectContent(content), location)
		{
		}

		private HttpResponseMessage(HttpStatusCode? statusCode, HttpContent content, Uri location)
		{
			Message = new HttpResponseMessage();

			if (content != null)
				Message.Content = content;

			if (statusCode != null)
				Message.StatusCode = statusCode.Value;

			if (location != null)
				Message.Headers.Location = location;
		}

		internal HttpResponseMessage Message { get; set; }
	}
}