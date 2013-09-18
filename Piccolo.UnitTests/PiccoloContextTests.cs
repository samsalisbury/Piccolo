using System.IO;
using System.Text;
using System.Web;
using NSubstitute;
using NUnit.Framework;
using Shouldly;

namespace Piccolo.UnitTests
{
	public class PiccoloContextTests
	{
		[TestFixture]
		public class when_payload_is_retrieved_from_input_stream
		{
			[Test]
			public void it_is_cached_for_subsequent_invocations()
			{
				var httpContext = Substitute.For<HttpContextBase>();
				httpContext.Request.InputStream.Returns(new MemoryStream(Encoding.UTF8.GetBytes("TEST")));

				var context = new PiccoloContext(httpContext);

				context.RequestPayload.ShouldNotBeEmpty();
				context.RequestPayload.ShouldNotBeEmpty();
			}
		}
	}
}