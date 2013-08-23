using System;
using System.IO;
using System.Web;

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