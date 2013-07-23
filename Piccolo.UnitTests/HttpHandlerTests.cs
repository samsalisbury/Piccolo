using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using Moq;
using NUnit.Framework;
using Piccolo.Configuration;
using Piccolo.Request;
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
		public class when_handling_get_request_to_test_resource : given_http_handler
		{
			private HttpResponseMessage _responseMessage;

			[SetUp]
			public void SetUp()
			{
				var requestContext = new Mock<IRequestContextWrapper>();
				requestContext.SetupGet(x => x.Verb).Returns("GET");
				requestContext.SetupGet(x => x.Uri).Returns(new Uri("https://api.com/test-resources/1"));
				_responseMessage = HttpHandler.HandleRequest(requestContext.Object);
			}

			[Test]
			public void it_should_return_status_200()
			{
				_responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
			}

			[Test]
			public void it_should_return_status_reason_ok()
			{
				_responseMessage.ReasonPhrase.ShouldBe("OK");
			}

			[Test]
			public void it_should_return_content()
			{
				_responseMessage.Content.ReadAsStringAsync().Result.ShouldBe("TEST");
			}

			[Route("/test-resources/{id}")]
			public class GetTestResourceById : IGet<string>
			{
				public HttpResponseMessage<string> Get()
				{
					return Response.Success.Ok("TEST");
				}

				public int Id { get; set; }
			}
		}

		[TestFixture]
		public class when_handling_post_request_to_test_resource : given_http_handler
		{
			private HttpResponseMessage _responseMessage;

			[SetUp]
			public void SetUp()
			{
				var requestContext = new Mock<IRequestContextWrapper>();
				requestContext.SetupGet(x => x.Verb).Returns("POST");
				requestContext.SetupGet(x => x.Uri).Returns(new Uri("https://api.com/test-resources"));
				requestContext.SetupGet(x => x.Payload).Returns("{}");
				_responseMessage = HttpHandler.HandleRequest(requestContext.Object);
			}

			[Test]
			public void it_should_return_status_201()
			{
				_responseMessage.StatusCode.ShouldBe(HttpStatusCode.Created);
			}

			[Test]
			public void it_should_return_status_reason_no_content()
			{
				_responseMessage.ReasonPhrase.ShouldBe("Created");
			}

			[Test]
			public void it_should_not_return_content()
			{
				_responseMessage.Content.ShouldBe(null);
			}

			[Route("/test-resources")]
			public class CreateTestResource : IPost<CreateTestResource.Parameters>
			{
				public HttpResponseMessage<dynamic> Post(Parameters parameters)
				{
					return Response.Success.Created();
				}

				public class Parameters
				{
				}
			}
		}

		[TestFixture]
		public class when_handling_put_request_to_test_resource : given_http_handler
		{
			private HttpResponseMessage _responseMessage;

			[SetUp]
			public void SetUp()
			{
				var requestContext = new Mock<IRequestContextWrapper>();
				requestContext.SetupGet(x => x.Verb).Returns("PUT");
				requestContext.SetupGet(x => x.Uri).Returns(new Uri("https://api.com/test-resources/1"));
				requestContext.SetupGet(x => x.Payload).Returns("{}");
				_responseMessage = HttpHandler.HandleRequest(requestContext.Object);
			}

			[Test]
			public void it_should_return_status_204()
			{
				_responseMessage.StatusCode.ShouldBe(HttpStatusCode.NoContent);
			}

			[Test]
			public void it_should_return_status_reason_no_content()
			{
				_responseMessage.ReasonPhrase.ShouldBe("No Content");
			}

			[Test]
			public void it_should_not_return_content()
			{
				_responseMessage.Content.ShouldBe(null);
			}

			[Route("/test-resources/{Id}")]
			public class UpdateTestResource : IPut<UpdateTestResource.Parameters>
			{
				public HttpResponseMessage<dynamic> Put(Parameters parameters)
				{
					return Response.Success.NoContent();
				}

				public class Parameters
				{
				}

				public int Id { get; set; }
			}
		}

		[TestFixture]
		public class when_handling_delete_request_to_test_resource : given_http_handler
		{
			private HttpResponseMessage _responseMessage;

			[SetUp]
			public void SetUp()
			{
				var requestContext = new Mock<IRequestContextWrapper>();
				requestContext.SetupGet(x => x.Verb).Returns("DELETE");
				requestContext.SetupGet(x => x.Uri).Returns(new Uri("https://api.com/test-resources/1"));
				_responseMessage = HttpHandler.HandleRequest(requestContext.Object);
			}

			[Test]
			public void it_should_return_status_204()
			{
				_responseMessage.StatusCode.ShouldBe(HttpStatusCode.NoContent);
			}

			[Test]
			public void it_should_return_status_reason_no_content()
			{
				_responseMessage.ReasonPhrase.ShouldBe("No Content");
			}

			[Test]
			public void it_should_not_return_content()
			{
				_responseMessage.Content.ShouldBe(null);
			}

			[Route("/test-resources/{Id}")]
			public class DeleteTestResource : IDelete
			{
				public HttpResponseMessage<dynamic> Delete()
				{
					return Response.Success.NoContent();
				}

				public int Id { get; set; }
			}
		}

		[TestFixture]
		public class when_handling_request_with_unsupported_verb : given_http_handler
		{
			private HttpResponseMessage _responseMessage;

			[SetUp]
			public void SetUp()
			{
				var requestContext = new Mock<IRequestContextWrapper>();
				requestContext.SetupGet(x => x.Verb).Returns("FAKE!");
				requestContext.SetupGet(x => x.Uri).Returns(new Uri("https://api.com/"));
				_responseMessage = HttpHandler.HandleRequest(requestContext.Object);
			}

			[Test]
			public void it_should_return_status_404()
			{
				_responseMessage.StatusCode.ShouldBe(HttpStatusCode.NotFound);
			}

			[Test]
			public void it_should_return_status_reason_method_not_allowed()
			{
				_responseMessage.ReasonPhrase.ShouldBe("Not Found");
			}

			[Test]
			public void it_should_not_return_content()
			{
				_responseMessage.Content.ShouldBe(null);
			}
		}

		[TestFixture]
		public class when_handling_request_to_unhandled_resource : given_http_handler
		{
			private HttpResponseMessage _responseMessage;

			[SetUp]
			public void SetUp()
			{
				var requestContext = new Mock<IRequestContextWrapper>();
				requestContext.SetupGet(x => x.Verb).Returns("GET");
				requestContext.SetupGet(x => x.Uri).Returns(new Uri("https://api.com/unhandled/resource"));
				_responseMessage = HttpHandler.HandleRequest(requestContext.Object);
			}

			[Test]
			public void it_should_return_status_404()
			{
				_responseMessage.StatusCode.ShouldBe(HttpStatusCode.NotFound);
			}

			[Test]
			public void it_should_return_status_reason_not_found()
			{
				_responseMessage.ReasonPhrase.ShouldBe("Not Found");
			}

			[Test]
			public void it_should_not_return_content()
			{
				_responseMessage.Content.ShouldBe(null);
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