using System;
using System.Net.Http;
using System.Reflection;
using Moq;
using NUnit.Framework;
using Piccolo.Abstractions;
using Piccolo.IoC;
using Piccolo.Routing;
using Shouldly;

namespace Piccolo.UnitTests
{
	public class HttpHandlerTests
	{
		[TestFixture]
		public class when_instantiated : given_http_handler
		{
			[Test]
			public void it_should_configure_request_handler_factory()
			{
				HttpHandler.Configuration.RequestHandlerFactory.ShouldNotBe(null);
			}

			[Test]
			public void it_should_configure_request_handlers()
			{
				HttpHandler.Configuration.RequestHandlers.Count.ShouldBeGreaterThan(0);
			}

			[Test]
			public void it_should_configure_router()
			{
				HttpHandler.Configuration.Router.ShouldNotBe(null);
			}
		}

		[TestFixture]
		public class when_handling_request_to_test_resource : given_http_handler
		{
			private HttpResponseMessage _responseMessage;

			[SetUp]
			public void SetUp()
			{
				var requestContext = new Mock<IRequestContextWrapper>();
				requestContext.SetupGet(x => x.Uri).Returns(new Uri("https://api.com/test-resource/1"));
				_responseMessage = HttpHandler.HandleRequest(requestContext.Object);
			}

			[Test]
			public void it_should_return_content()
			{
				_responseMessage.Content.ReadAsStringAsync().Result.ShouldBe("TEST");
			}

			[Route("/test-resource/{id}")]
			public class GetTestResourceById : IGet<string>
			{
				public string Get()
				{
					return "TEST";
				}

				public int Id { get; set; }
			}
		}

		public abstract class given_http_handler
		{
			protected HttpHandler HttpHandler;

			protected given_http_handler()
			{
				HttpHandler = new HttpHandler(true, Assembly.GetExecutingAssembly());
				HttpHandler.Configuration.RequestHandlerFactory = new DefaultRequestHandlerFactory();
			}
		}
	}
}