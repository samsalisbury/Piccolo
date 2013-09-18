using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Web;
using Piccolo.Internal;

namespace Piccolo
{
	public class PiccoloContext
	{
		private readonly HttpContextBase _httpContext;
		private string _requestPayload;

		public PiccoloContext(HttpContextBase httpContext)
		{
			_httpContext = httpContext;
			Data = new ExpandoObject();
		}

		public HttpContextBase Http
		{
			get { return _httpContext; }
		}

		public string RequestVerb
		{
			get { return _httpContext.Request.HttpMethod; }
		}

		public Uri RequestUri
		{
			get { return _httpContext.Request.Url; }
		}

		public IDictionary<string, string> RequestQueryParameters
		{
			get { return HttpUtility.ParseQueryString(RequestUri.Query).ToDictionary(); }
		}

		public string RequestPayload
		{
			get
			{
				if (_requestPayload == null)
				{
					if (_httpContext.Request.InputStream.CanRead == false)
						return string.Empty;

					using (var reader = new StreamReader(_httpContext.Request.InputStream))
						_requestPayload = reader.ReadToEnd();
				}

				return _requestPayload;
			}
		}

		public dynamic Data { get; private set; }
	}
}