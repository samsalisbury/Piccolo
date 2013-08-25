using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Piccolo.Internal;

namespace Piccolo
{
	public class PiccoloContext
	{
		private readonly HttpContextBase _httpContext;

		public PiccoloContext(HttpContextBase httpContext)
		{
			_httpContext = httpContext;
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
				if (_httpContext.Request.InputStream.CanRead == false)
					return string.Empty;

				using (var reader = new StreamReader(_httpContext.Request.InputStream))
					return reader.ReadToEnd();
			}
		}
	}
}