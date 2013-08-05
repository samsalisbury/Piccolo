using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Web;

namespace Piccolo.Request
{
	[ExcludeFromCodeCoverage]
	public class RequestContextWrapper : IRequestContextWrapper
	{
		private readonly HttpContext _context;

		public RequestContextWrapper(HttpContext context)
		{
			_context = context;
		}

		public string Verb
		{
			get { return _context.Request.HttpMethod; }
		}

		public Uri Uri
		{
			get { return _context.Request.Url; }
		}

		public string Payload
		{
			get
			{
				if (_context.Request.InputStream.CanRead == false)
					return string.Empty;

				using (var reader = new StreamReader(_context.Request.InputStream))
					return reader.ReadToEnd();
			}
		}
	}
}