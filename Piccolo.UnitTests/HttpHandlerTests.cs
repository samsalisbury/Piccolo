using NUnit.Framework;
using Shouldly;

namespace Piccolo.UnitTests
{
	public class HttpHandlerTests
	{
		[TestFixture]
		public class when_instantiated : given_http_handler
		{
			[Test]
			public void it_should_configure_custom_request_handler_factory()
			{
				HttpHandler.Configuration.RequestHandlerFactory.ShouldNotBe(null);
			}
		}

		public abstract class given_http_handler
		{
			protected HttpHandler HttpHandler;

			protected given_http_handler()
			{
				HttpHandler = new HttpHandler();
			}
		}
	}
}