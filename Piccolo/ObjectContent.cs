using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Piccolo
{
	public class ObjectContent : HttpContent
	{
		public ObjectContent(object content)
		{
			Content = content;
		}

		public object Content { get; private set; }

		[ExcludeFromCodeCoverage]
		protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
		{
			throw new NotSupportedException();
		}

		[ExcludeFromCodeCoverage]
		protected override bool TryComputeLength(out long length)
		{
			throw new NotSupportedException();
		}
	}
}