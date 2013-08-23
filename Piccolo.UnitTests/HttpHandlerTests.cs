using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using Moq;
using NUnit.Framework;
using Piccolo.Configuration;

namespace Piccolo.UnitTests
{
	public class HttpHandlerTests
	{
		[TestFixture]
		public class when_processing_get_request_to_test_resource : given_http_handler
		{
			private Mock<HttpResponseBase> _httpResponse;

			[SetUp]
			public void SetUp()
			{
				_httpResponse = new Mock<HttpResponseBase>();

				var httpContext = new Mock<HttpContextBase>();
				httpContext.SetupGet(x => x.Request.HttpMethod).Returns("GET");
				httpContext.SetupGet(x => x.Request.Url).Returns(new Uri("https://api.com/test-resources/1?test=true"));
				httpContext.SetupGet(x => x.Request.InputStream.CanRead).Returns(false);
				httpContext.SetupGet(x => x.Response).Returns(_httpResponse.Object);
				PiccoloHttpHandler.ProcessRequest(httpContext.Object);
			}

			[Test]
			public void it_should_return_status_200()
			{
				_httpResponse.VerifySet(x => x.StatusCode = (int)HttpStatusCode.OK);
			}

			[Test]
			public void it_should_return_status_reason_ok()
			{
				_httpResponse.VerifySet(x => x.StatusDescription = "OK");
			}

			[Test]
			public void it_should_set_mime_type_to_application_json()
			{
				_httpResponse.Verify(x => x.AddHeader("Content-Type", "application/json"));
			}

			[Test]
			public void it_should_return_content()
			{
				_httpResponse.Verify(x => x.Write("{\"test\":\"Test\"}"));
			}

			[Route("/test-resources/{id}")]
			public class GetTestResourceById : IGet<GetTestResourceById.Model>
			{
				public HttpResponseMessage<Model> Get()
				{
					return Response.Success.Ok(new Model {Test = "Test"});
				}

				public int Id { get; set; }

				public class Model
				{
					public string Test { get; set; }
				}
			}
		}

		[TestFixture]
		public class when_processing_post_request_to_test_resource : given_http_handler
		{
			private Mock<HttpResponseBase> _httpResponse;

			[SetUp]
			public void SetUp()
			{
				_httpResponse = new Mock<HttpResponseBase>();

				var httpContext = new Mock<HttpContextBase>();
				httpContext.SetupGet(x => x.Request.HttpMethod).Returns("POST");
				httpContext.SetupGet(x => x.Request.Url).Returns(new Uri("https://api.com/test-resources"));
				httpContext.SetupGet(x => x.Request.InputStream).Returns(new MemoryStream(Encoding.UTF8.GetBytes("{\"Test\":\"Test\"}")));
				httpContext.SetupGet(x => x.Response).Returns(_httpResponse.Object);
				PiccoloHttpHandler.ProcessRequest(httpContext.Object);
			}

			[Test]
			public void it_should_return_status_201()
			{
				_httpResponse.VerifySet(x => x.StatusCode = (int)HttpStatusCode.Created);
			}

			[Test]
			public void it_should_return_status_reason_no_content()
			{
				_httpResponse.VerifySet(x => x.StatusDescription = "Created");
			}

			[Test]
			public void it_should_set_mime_type_to_application_json()
			{
				_httpResponse.Verify(x => x.AddHeader("Content-Type", "application/json"));
			}

			[Test]
			public void it_should_return_content()
			{
				_httpResponse.Verify(x => x.Write("{\"test\":\"Test\"}"));
			}

			[Route("/test-resources")]
			public class CreateTestResource : IPost<CreateTestResource.Parameters, CreateTestResource.Parameters>
			{
				public HttpResponseMessage<Parameters> Post(Parameters parameters)
				{
					return Response.Success.Created(parameters);
				}

				public class Parameters
				{
					public string Test { get; set; }
				}
			}
		}

		[TestFixture]
		public class when_processing_put_request_to_test_resource : given_http_handler
		{
			private Mock<HttpResponseBase> _httpResponse;

			[SetUp]
			public void SetUp()
			{
				_httpResponse = new Mock<HttpResponseBase>();

				var httpContext = new Mock<HttpContextBase>();
				httpContext.SetupGet(x => x.Request.HttpMethod).Returns("PUT");
				httpContext.SetupGet(x => x.Request.Url).Returns(new Uri("https://api.com/test-resources/1"));
				httpContext.SetupGet(x => x.Request.InputStream).Returns(new MemoryStream(Encoding.UTF8.GetBytes("{\"Test\":\"Test\"}")));
				httpContext.SetupGet(x => x.Response).Returns(_httpResponse.Object);
				PiccoloHttpHandler.ProcessRequest(httpContext.Object);
			}

			[Test]
			public void it_should_return_status_204()
			{
				_httpResponse.VerifySet(x => x.StatusCode = (int)HttpStatusCode.OK);
			}

			[Test]
			public void it_should_return_status_reason_no_content()
			{
				_httpResponse.VerifySet(x => x.StatusDescription = "OK");
			}

			[Test]
			public void it_should_set_mime_type_to_application_json()
			{
				_httpResponse.Verify(x => x.AddHeader("Content-Type", "application/json"));
			}

			[Test]
			public void it_should_return_content()
			{
				_httpResponse.Verify(x => x.Write("{\"test\":\"Test\"}"));
			}

			[Route("/test-resources/{Id}")]
			public class UpdateTestResource : IPut<UpdateTestResource.Parameters, UpdateTestResource.Parameters>
			{
				public HttpResponseMessage<Parameters> Put(Parameters task)
				{
					return Response.Success.Ok(task);
				}

				public class Parameters
				{
					public string Test { get; set; }
				}

				public int Id { get; set; }
			}
		}

		[TestFixture]
		public class when_processing_delete_request_to_test_resource : given_http_handler
		{
			private Mock<HttpResponseBase> _httpResponse;

			[SetUp]
			public void SetUp()
			{
				_httpResponse = new Mock<HttpResponseBase>();

				var httpContext = new Mock<HttpContextBase>();
				httpContext.SetupGet(x => x.Request.HttpMethod).Returns("DELETE");
				httpContext.SetupGet(x => x.Request.Url).Returns(new Uri("https://api.com/test-resources/1"));
				httpContext.SetupGet(x => x.Request.InputStream.CanRead).Returns(false);
				httpContext.SetupGet(x => x.Response).Returns(_httpResponse.Object);
				PiccoloHttpHandler.ProcessRequest(httpContext.Object);
			}

			[Test]
			public void it_should_return_status_204()
			{
				_httpResponse.VerifySet(x => x.StatusCode = (int)HttpStatusCode.NoContent);
			}

			[Test]
			public void it_should_return_status_reason_no_content()
			{
				_httpResponse.VerifySet(x => x.StatusDescription = "No Content");
			}

			[Test]
			public void it_should_not_return_content()
			{
				_httpResponse.Verify(x => x.Write(It.IsAny<string>()), Times.Never());
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
		public class when_processing_request_with_unsupported_verb : given_http_handler
		{
			private Mock<HttpResponseBase> _httpResponse;

			[SetUp]
			public void SetUp()
			{
				_httpResponse = new Mock<HttpResponseBase>();

				var httpContext = new Mock<HttpContextBase>();
				httpContext.SetupGet(x => x.Request.HttpMethod).Returns("FAKE!");
				httpContext.SetupGet(x => x.Request.Url).Returns(new Uri("https://api.com/"));
				httpContext.SetupGet(x => x.Request.InputStream.CanRead).Returns(false);
				httpContext.SetupGet(x => x.Response).Returns(_httpResponse.Object);
				PiccoloHttpHandler.ProcessRequest(httpContext.Object);
			}

			[Test]
			public void it_should_return_status_404()
			{
				_httpResponse.VerifySet(x => x.StatusCode = (int)HttpStatusCode.NotFound);
			}

			[Test]
			public void it_should_return_status_reason_method_not_allowed()
			{
				_httpResponse.VerifySet(x => x.StatusDescription = "Not Found");
			}

			[Test]
			public void it_should_not_return_content()
			{
				_httpResponse.Verify(x => x.Write(It.IsAny<string>()), Times.Never());
			}
		}

		[TestFixture]
		public class when_processing_request_to_unhandled_resource : given_http_handler
		{
			private Mock<HttpResponseBase> _httpResponse;

			[SetUp]
			public void SetUp()
			{
				_httpResponse = new Mock<HttpResponseBase>();

				var httpContext = new Mock<HttpContextBase>();
				httpContext.SetupGet(x => x.Request.HttpMethod).Returns("GET");
				httpContext.SetupGet(x => x.Request.Url).Returns(new Uri("https://api.com/unhandled/resource"));
				httpContext.SetupGet(x => x.Request.InputStream.CanRead).Returns(false);
				httpContext.SetupGet(x => x.Response).Returns(_httpResponse.Object);
				PiccoloHttpHandler.ProcessRequest(httpContext.Object);
			}

			[Test]
			public void it_should_return_status_404()
			{
				_httpResponse.VerifySet(x => x.StatusCode = (int)HttpStatusCode.NotFound);
			}

			[Test]
			public void it_should_return_status_reason_not_found()
			{
				_httpResponse.VerifySet(x => x.StatusDescription = "Not Found");
			}

			[Test]
			public void it_should_not_return_content()
			{
				_httpResponse.Verify(x => x.Write(It.IsAny<string>()), Times.Never());
			}
		}

		public abstract class given_http_handler
		{
			protected PiccoloHttpHandler PiccoloHttpHandler;

			protected given_http_handler()
			{
				PiccoloHttpHandler = new PiccoloHttpHandler(Assembly.GetExecutingAssembly(), false);
			}
		}
	}
}