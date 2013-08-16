using System;
using System.Web;
using NUnit.Framework;
using Shouldly;

namespace Piccolo.UnitTests.InvalidConfigurationDetection
{
	public class HttpHandlerTests
	{
		[TestFixture]
		public class when_initialising_http_handler_with_invalid_assembly
		{
			[Test]
			public void it_should_throw_exception()
			{
				Should.Throw<InvalidOperationException>(() => new HttpHandler(false, typeof(HttpApplication).Assembly));
			}
		}
	}
}