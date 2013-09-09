using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using Moq;
using NUnit.Framework;

namespace Piccolo.UnitTests
{
	public class PiccoloHttpHandlerTests
	{
		#region Verbs: GET, POST, PUT, DELETE

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
				httpContext.SetupGet(x => x.Request.Url).Returns(new Uri("https://api.com/test-resources/1?optionalParam=2"));
				httpContext.SetupGet(x => x.Request.InputStream.CanRead).Returns(false);
				httpContext.SetupGet(x => x.Response).Returns(_httpResponse.Object);

				var piccoloContext = new PiccoloContext(httpContext.Object);
				piccoloContext.Data.ContextualParam = 3;

				PiccoloHttpHandler.ProcessRequest(piccoloContext);
			}

			[Test]
			public void it_should_raise_request_processing_event()
			{
				_httpResponse.Verify(x => x.Write("RequestProcessingEvent handled"));
			}

			[Test]
			public void it_should_raise_request_processed_event()
			{
				_httpResponse.Verify(x => x.Write("RequestProcessedEvent handled"));
			}

			[Test]
			public void it_should_not_raise_request_faulted_event()
			{
				_httpResponse.Verify(x => x.Write("RequestFaultedEvent handled"), Times.Never());
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
				_httpResponse.VerifySet(x => x.ContentType = "application/json");
			}

			[Test]
			public void it_should_return_content()
			{
				_httpResponse.Verify(x => x.Write("{\"test\":6}"));
			}

			[Route("/test-resources/{id}")]
			public class GetTestResourceById : IGet<GetTestResourceById.Model>
			{
				public HttpResponseMessage<Model> Get()
				{
					return Response.Success.Ok(new Model {Test = Id + OptionalParam + ContextualParam});
				}

				public int Id { get; set; }

				[Optional]
				public int OptionalParam { get; set; }

				[Contextual]
				public int ContextualParam { get; set; }

				public class Model
				{
					public int Test { get; set; }
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
				PiccoloHttpHandler.ProcessRequest(new PiccoloContext(httpContext.Object));
			}

			[Test]
			public void it_should_raise_request_processing_event()
			{
				_httpResponse.Verify(x => x.Write("RequestProcessingEvent handled"));
			}

			[Test]
			public void it_should_raise_request_processed_event()
			{
				_httpResponse.Verify(x => x.Write("RequestProcessedEvent handled"));
			}

			[Test]
			public void it_should_not_raise_request_faulted_event()
			{
				_httpResponse.Verify(x => x.Write("RequestFaultedEvent handled"), Times.Never());
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
				_httpResponse.VerifySet(x => x.ContentType = "application/json");
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
				PiccoloHttpHandler.ProcessRequest(new PiccoloContext(httpContext.Object));
			}

			[Test]
			public void it_should_raise_request_processing_event()
			{
				_httpResponse.Verify(x => x.Write("RequestProcessingEvent handled"));
			}

			[Test]
			public void it_should_raise_request_processed_event()
			{
				_httpResponse.Verify(x => x.Write("RequestProcessedEvent handled"));
			}

			[Test]
			public void it_should_not_raise_request_faulted_event()
			{
				_httpResponse.Verify(x => x.Write("RequestFaultedEvent handled"), Times.Never());
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
				_httpResponse.VerifySet(x => x.ContentType = "application/json");
			}

			[Test]
			public void it_should_return_content()
			{
				_httpResponse.Verify(x => x.Write("{\"test\":\"Test\"}"));
			}

			[Route("/test-resources/{Id}")]
			public class UpdateTestResource : IPut<UpdateTestResource.Parameters, UpdateTestResource.Parameters>
			{
				public HttpResponseMessage<Parameters> Put(Parameters parameters)
				{
					return Response.Success.Ok(parameters);
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
				PiccoloHttpHandler.ProcessRequest(new PiccoloContext(httpContext.Object));
			}

			[Test]
			public void it_should_raise_request_processing_event()
			{
				_httpResponse.Verify(x => x.Write("RequestProcessingEvent handled"));
			}

			[Test]
			public void it_should_raise_request_processed_event()
			{
				_httpResponse.Verify(x => x.Write("RequestProcessedEvent handled"));
			}

			[Test]
			public void it_should_not_raise_request_faulted_event()
			{
				_httpResponse.Verify(x => x.Write("RequestFaultedEvent handled"), Times.Never());
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
				_httpResponse.Verify(x => x.Write(It.Is((string value) => !value.Contains("Event"))), Times.Never());
			}

			[Route("/test-resources/{Id}")]
			public class DeleteTestResource : IDelete<string, string>
			{
				public HttpResponseMessage<string> Delete(string parameters)
				{
					return Response.Success.NoContent<string>();
				}

				public int Id { get; set; }
			}
		}

		#endregion

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
				PiccoloHttpHandler.ProcessRequest(new PiccoloContext(httpContext.Object));
			}

			[Test]
			public void it_should_raise_request_processing_event()
			{
				_httpResponse.Verify(x => x.Write("RequestProcessingEvent handled"));
			}

			[Test]
			public void it_should_raise_request_processed_event()
			{
				_httpResponse.Verify(x => x.Write("RequestProcessedEvent handled"));
			}

			[Test]
			public void it_should_not_raise_request_faulted_event()
			{
				_httpResponse.Verify(x => x.Write("RequestFaultedEvent handled"), Times.Never());
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
				_httpResponse.Verify(x => x.Write(It.Is((string value) => !value.Contains("Event"))), Times.Never());
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
				PiccoloHttpHandler.ProcessRequest(new PiccoloContext(httpContext.Object));
			}

			[Test]
			public void it_should_raise_request_processing_event()
			{
				_httpResponse.Verify(x => x.Write("RequestProcessingEvent handled"));
			}

			[Test]
			public void it_should_raise_request_processed_event()
			{
				_httpResponse.Verify(x => x.Write("RequestProcessedEvent handled"));
			}

			[Test]
			public void it_should_not_raise_request_faulted_event()
			{
				_httpResponse.Verify(x => x.Write("RequestFaultedEvent handled"), Times.Never());
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
				_httpResponse.Verify(x => x.Write(It.Is((string value) => !value.Contains("Event"))), Times.Never());
			}
		}

		[TestFixture]
		public class when_processing_request_with_runtime_exception : given_http_handler
		{
			private Mock<HttpResponseBase> _httpResponse;

			[SetUp]
			public void SetUp()
			{
				_httpResponse = new Mock<HttpResponseBase>();

				var httpContext = new Mock<HttpContextBase>();
				httpContext.SetupGet(x => x.Request.HttpMethod).Returns("GET");
				httpContext.SetupGet(x => x.Request.Url).Returns(new Uri("https://api.com/exception"));
				httpContext.SetupGet(x => x.Request.InputStream.CanRead).Returns(false);
				httpContext.SetupGet(x => x.Response).Returns(_httpResponse.Object);
				PiccoloHttpHandler.ProcessRequest(new PiccoloContext(httpContext.Object));
			}

			[Test]
			public void it_should_raise_request_processing_event()
			{
				_httpResponse.Verify(x => x.Write("RequestProcessingEvent handled"));
			}

			[Test]
			public void it_should_raise_request_processed_event()
			{
				_httpResponse.Verify(x => x.Write("RequestProcessedEvent handled"));
			}

			[Test]
			public void it_should_raise_request_faulted_event()
			{
				_httpResponse.Verify(x => x.Write("RequestFaultedEvent handled"));
			}

			[Test]
			public void it_should_return_status_500()
			{
				_httpResponse.VerifySet(x => x.StatusCode = (int)HttpStatusCode.InternalServerError);
			}

			[Test]
			public void it_should_return_status_reason_ok()
			{
				_httpResponse.VerifySet(x => x.StatusDescription = "Internal Server Error");
			}

			[Test]
			public void it_should_not_return_content()
			{
				_httpResponse.Verify(x => x.Write(It.Is((string value) => !value.Contains("Event"))), Times.Never());
			}

			[Route("/exception")]
			public class HandlerWithRuntimeException : IGet<string>
			{
				public HttpResponseMessage<string> Get()
				{
					throw new Exception();
				}
			}
		}

		[TestFixture]
		public class when_processing_request_with_route_parameter_datatype_mismatch_exception : given_http_handler
		{
			private Mock<HttpResponseBase> _httpResponse;

			[SetUp]
			public void SetUp()
			{
				_httpResponse = new Mock<HttpResponseBase>();

				var httpContext = new Mock<HttpContextBase>();
				httpContext.SetupGet(x => x.Request.HttpMethod).Returns("GET");
				httpContext.SetupGet(x => x.Request.Url).Returns(new Uri("https://api.com/route_parameter_datatype_mismatch_exception/test"));
				httpContext.SetupGet(x => x.Request.InputStream.CanRead).Returns(false);
				httpContext.SetupGet(x => x.Response).Returns(_httpResponse.Object);
				PiccoloHttpHandler.ProcessRequest(new PiccoloContext(httpContext.Object));
			}

			[Test]
			public void it_should_raise_request_processing_event()
			{
				_httpResponse.Verify(x => x.Write("RequestProcessingEvent handled"));
			}

			[Test]
			public void it_should_raise_request_processed_event()
			{
				_httpResponse.Verify(x => x.Write("RequestProcessedEvent handled"));
			}

			[Test]
			public void it_should_raise_request_faulted_event()
			{
				_httpResponse.Verify(x => x.Write("RequestFaultedEvent handled"));
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
				_httpResponse.Verify(x => x.Write(It.Is((string value) => !value.Contains("Event"))), Times.Never());
			}

			[Route("/route_parameter_datatype_mismatch_exception/{id}")]
			public class HandlerWithRuntimeRouteParameterDatatypeMismatchException : IGet<string>
			{
				public int Id { get; set; }

				[ExcludeFromCodeCoverage]
				public HttpResponseMessage<string> Get()
				{
					return Response.Success.NoContent<string>();
				}
			}
		}

		[TestFixture]
		public class when_processing_request_with_route_parameter_datatype_mismatch_exception_in_aspnet_debug_mode : given_http_handler
		{
			private Mock<HttpResponseBase> _httpResponse;

			[SetUp]
			public void SetUp()
			{
				_httpResponse = new Mock<HttpResponseBase>();

				var httpContext = new Mock<HttpContextBase>();
				httpContext.SetupGet(x => x.IsDebuggingEnabled).Returns(true);
				httpContext.SetupGet(x => x.Request.HttpMethod).Returns("GET");
				httpContext.SetupGet(x => x.Request.Url).Returns(new Uri("https://api.com/route_parameter_datatype_mismatch_exception/test"));
				httpContext.SetupGet(x => x.Request.InputStream.CanRead).Returns(false);
				httpContext.SetupGet(x => x.Response).Returns(_httpResponse.Object);
				PiccoloHttpHandler.ProcessRequest(new PiccoloContext(httpContext.Object));
			}

			[Test]
			public void it_should_raise_request_processing_event()
			{
				_httpResponse.Verify(x => x.Write("RequestProcessingEvent handled"));
			}

			[Test]
			public void it_should_raise_request_processed_event()
			{
				_httpResponse.Verify(x => x.Write("RequestProcessedEvent handled"));
			}

			[Test]
			public void it_should_raise_request_faulted_event()
			{
				_httpResponse.Verify(x => x.Write("RequestFaultedEvent handled"));
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
			public void it_should_return_content()
			{
				_httpResponse.Verify(x => x.Write(It.Is<string>(s => s.Contains("Exception"))));
			}
		}

		[TestFixture]
		public class when_processing_request_with_malformed_payload_exception : given_http_handler
		{
			private Mock<HttpResponseBase> _httpResponse;

			[SetUp]
			public void SetUp()
			{
				_httpResponse = new Mock<HttpResponseBase>();

				var httpContext = new Mock<HttpContextBase>();
				httpContext.SetupGet(x => x.Request.HttpMethod).Returns("POST");
				httpContext.SetupGet(x => x.Request.Url).Returns(new Uri("https://api.com/malformed_payload_exception"));
				httpContext.SetupGet(x => x.Request.InputStream).Returns(new MemoryStream(Encoding.UTF8.GetBytes("invalid")));
				httpContext.SetupGet(x => x.Response).Returns(_httpResponse.Object);
				PiccoloHttpHandler.ProcessRequest(new PiccoloContext(httpContext.Object));
			}

			[Test]
			public void it_should_raise_request_processing_event()
			{
				_httpResponse.Verify(x => x.Write("RequestProcessingEvent handled"));
			}

			[Test]
			public void it_should_raise_request_processed_event()
			{
				_httpResponse.Verify(x => x.Write("RequestProcessedEvent handled"));
			}

			[Test]
			public void it_should_raise_request_faulted_event()
			{
				_httpResponse.Verify(x => x.Write("RequestFaultedEvent handled"));
			}

			[Test]
			public void it_should_return_status_422()
			{
				_httpResponse.VerifySet(x => x.StatusCode = 422);
			}

			[Test]
			public void it_should_return_status_reason_unprocessable_entity()
			{
				_httpResponse.VerifySet(x => x.StatusDescription = "Unprocessable Entity");
			}

			[Test]
			public void it_should_not_return_content()
			{
				_httpResponse.Verify(x => x.Write(It.Is((string value) => !value.Contains("Event"))), Times.Never());
			}

			[Route("/malformed_payload_exception")]
			public class HandlerWithRuntimeRouteParameterDatatypeMismatchException : IPost<DateTime, string>
			{
				[ExcludeFromCodeCoverage]
				public HttpResponseMessage<string> Post(DateTime parameters)
				{
					return Response.Success.NoContent<string>();
				}
			}
		}

		[TestFixture]
		public class when_processing_request_with_runtime_exception_in_aspnet_debug_mode : given_http_handler
		{
			private Mock<HttpResponseBase> _httpResponse;

			[SetUp]
			public void SetUp()
			{
				_httpResponse = new Mock<HttpResponseBase>();

				var httpContext = new Mock<HttpContextBase>();
				httpContext.SetupGet(x => x.IsDebuggingEnabled).Returns(true);
				httpContext.SetupGet(x => x.Request.HttpMethod).Returns("GET");
				httpContext.SetupGet(x => x.Request.Url).Returns(new Uri("https://api.com/exception"));
				httpContext.SetupGet(x => x.Request.InputStream.CanRead).Returns(false);
				httpContext.SetupGet(x => x.Response).Returns(_httpResponse.Object);
				PiccoloHttpHandler.ProcessRequest(new PiccoloContext(httpContext.Object));
			}

			[Test]
			public void it_should_raise_request_processing_event()
			{
				_httpResponse.Verify(x => x.Write("RequestProcessingEvent handled"));
			}

			[Test]
			public void it_should_raise_request_processed_event()
			{
				_httpResponse.Verify(x => x.Write("RequestProcessedEvent handled"));
			}

			[Test]
			public void it_should_raise_request_faulted_event()
			{
				_httpResponse.Verify(x => x.Write("RequestFaultedEvent handled"));
			}

			[Test]
			public void it_should_return_status_500()
			{
				_httpResponse.VerifySet(x => x.StatusCode = (int)HttpStatusCode.InternalServerError);
			}

			[Test]
			public void it_should_return_status_reason_ok()
			{
				_httpResponse.VerifySet(x => x.StatusDescription = "Internal Server Error");
			}

			[Test]
			public void it_should_return_content()
			{
				_httpResponse.Verify(x => x.Write(It.Is<string>(s => s.Contains("Exception"))));
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