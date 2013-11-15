using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using NSubstitute;
using NUnit.Framework;
using Piccolo.Configuration;
using Piccolo.UnitTests.Events;
using Piccolo.UnitTests.Request;
using Piccolo.Validation;

namespace Piccolo.UnitTests
{
	public class PiccoloHttpHandlerTests
	{
		#region Verbs: GET, POST, PUT, DELETE

		[TestFixture]
		public class when_processing_get_request_to_test_resource : given_http_handler
		{
			private HttpResponseBase _httpResponse;

			[SetUp]
			public void SetUp()
			{
				_httpResponse = Substitute.For<HttpResponseBase>();

				var inputStream = Substitute.For<Stream>();
				inputStream.CanRead.Returns(false);

				var httpContext = Substitute.For<HttpContextBase>();
				httpContext.Request.HttpMethod.Returns("GET");
				httpContext.Request.Url.Returns(new Uri("https://api.com/test-resources/1?optionalParam=2"));
				httpContext.Request.InputStream.Returns(inputStream);
				httpContext.Response.Returns(_httpResponse);

				var piccoloContext = new PiccoloContext(httpContext);
				piccoloContext.Data.ContextualParam = 3;

				PiccoloHttpHandler.ProcessRequest(piccoloContext);
			}

			[Test]
			public void it_should_raise_request_processing_event()
			{
				_httpResponse.Received().Write("RequestProcessingEvent handled");
			}

			[Test]
			public void it_should_raise_request_processed_event()
			{
				_httpResponse.Received().Write("RequestProcessedEvent handled with StopEventProcessing: {\"test\":6}");
			}

			[Test]
			public void it_should_not_raise_request_faulted_event()
			{
				_httpResponse.DidNotReceive().Write("RequestFaultedEvent handled");
			}

			[Test]
			public void it_should_return_status_200()
			{
				_httpResponse.Received().StatusCode = (int)HttpStatusCode.OK;
			}

			[Test]
			public void it_should_return_status_reason_ok()
			{
				_httpResponse.Received().StatusDescription = "OK";
			}

			[Test]
			public void it_should_set_mime_type_to_application_json()
			{
				_httpResponse.Received().ContentType = "application/json";
			}

			[Test]
			public void it_should_return_content()
			{
				_httpResponse.Received().Write("{\"test\":6}");
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

				[Contextual]
				public int ContextualParam2 { get; set; }

				public class Model
				{
					public int Test { get; set; }
				}
			}
		}

		[TestFixture]
		public class when_processing_post_request_to_test_resource : given_http_handler
		{
			private HttpResponseBase _httpResponse;

			[SetUp]
			public void SetUp()
			{
				_httpResponse = Substitute.For<HttpResponseBase>();

				var httpContext = Substitute.For<HttpContextBase>();
				httpContext.Request.HttpMethod.Returns("POST");
				httpContext.Request.Url.Returns(new Uri("https://api.com/test-resources"));
				httpContext.Request.InputStream.Returns(new MemoryStream(Encoding.UTF8.GetBytes("{\"Test\":\"Test\"}")));
				httpContext.Response.Returns(_httpResponse);

				PiccoloHttpHandler.ProcessRequest(new PiccoloContext(httpContext));
			}

			[Test]
			public void it_should_raise_request_processing_event()
			{
				_httpResponse.Received().Write("RequestProcessingEvent handled");
			}

			[Test]
			public void it_should_raise_request_processed_event()
			{
				_httpResponse.Received().Write("RequestProcessedEvent handled with StopEventProcessing: {\"test\":\"Test\"}");
			}

			[Test]
			public void it_should_not_raise_request_faulted_event()
			{
				_httpResponse.DidNotReceive().Write("RequestFaultedEvent handled");
			}

			[Test]
			public void it_should_return_status_201()
			{
				_httpResponse.Received().StatusCode = (int)HttpStatusCode.Created;
			}

			[Test]
			public void it_should_return_status_reason_no_content()
			{
				_httpResponse.Received().StatusDescription = "Created";
			}

			[Test]
			public void it_should_set_mime_type_to_application_json()
			{
				_httpResponse.Received().ContentType = "application/json";
			}

			[Test]
			public void it_should_return_content()
			{
				_httpResponse.Received().Write("{\"test\":\"Test\"}");
			}

			[Test]
			public void it_should_have_location_header_set()
			{
				_httpResponse.Received().AddHeader("Location", "/created/resource");
			}

			[Route("/test-resources")]
			public class CreateTestResource : IPost<CreateTestResource.Parameters, CreateTestResource.Parameters>
			{
				public HttpResponseMessage<Parameters> Post(Parameters parameters)
				{
					return Response.Success.Created(parameters, new Uri("/created/resource", UriKind.Relative));
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
			private HttpResponseBase _httpResponse;

			[SetUp]
			public void SetUp()
			{
				_httpResponse = Substitute.For<HttpResponseBase>();

				var httpContext = Substitute.For<HttpContextBase>();
				httpContext.Request.HttpMethod.Returns("PUT");
				httpContext.Request.Url.Returns(new Uri("https://api.com/test-resources/1"));
				httpContext.Request.InputStream.Returns(new MemoryStream(Encoding.UTF8.GetBytes("{\"Test\":\"Test\"}")));
				httpContext.Response.Returns(_httpResponse);

				PiccoloHttpHandler.ProcessRequest(new PiccoloContext(httpContext));
			}

			[Test]
			public void it_should_raise_request_processing_event()
			{
				_httpResponse.Received().Write("RequestProcessingEvent handled");
			}

			[Test]
			public void it_should_raise_request_processed_event()
			{
				_httpResponse.Received().Write("RequestProcessedEvent handled with StopEventProcessing: {\"test\":\"Test\"}");
			}

			[Test]
			public void it_should_not_raise_request_faulted_event()
			{
				_httpResponse.DidNotReceive().Write("RequestFaultedEvent handled");
			}

			[Test]
			public void it_should_return_status_204()
			{
				_httpResponse.Received().StatusCode = (int)HttpStatusCode.OK;
			}

			[Test]
			public void it_should_return_status_reason_no_content()
			{
				_httpResponse.Received().StatusDescription = "OK";
			}

			[Test]
			public void it_should_set_mime_type_to_application_json()
			{
				_httpResponse.Received().ContentType = "application/json";
			}

			[Test]
			public void it_should_return_content()
			{
				_httpResponse.Received().Write("{\"test\":\"Test\"}");
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
			private HttpResponseBase _httpResponse;

			[SetUp]
			public void SetUp()
			{
				_httpResponse = Substitute.For<HttpResponseBase>();

				var httpContext = Substitute.For<HttpContextBase>();
				httpContext.Request.HttpMethod.Returns("DELETE");
				httpContext.Request.Url.Returns(new Uri("https://api.com/test-resources/1"));
				httpContext.Request.InputStream.Returns(new MemoryStream(Encoding.UTF8.GetBytes("\"\"")));
				httpContext.Response.Returns(_httpResponse);

				PiccoloHttpHandler.ProcessRequest(new PiccoloContext(httpContext));
			}

			[Test]
			public void it_should_raise_request_processing_event()
			{
				_httpResponse.Received().Write("RequestProcessingEvent handled");
			}

			[Test]
			public void it_should_raise_request_processed_event()
			{
				_httpResponse.Received().Write("RequestProcessedEvent handled with StopEventProcessing: ");
			}

			[Test]
			public void it_should_not_raise_request_faulted_event()
			{
				_httpResponse.DidNotReceive().Write("RequestFaultedEvent handled");
			}

			[Test]
			public void it_should_return_status_204()
			{
				_httpResponse.Received().StatusCode = (int)HttpStatusCode.NoContent;
			}

			[Test]
			public void it_should_return_status_reason_no_content()
			{
				_httpResponse.Received().StatusDescription = "No Content";
			}

			[Test]
			public void it_should_not_return_content()
			{
				_httpResponse.DidNotReceive().Write(Arg.Is<string>(x => !x.Contains("Event")));
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

		#region Exception Handling

		[TestFixture]
		public class when_processing_request_with_runtime_exception : given_http_handler
		{
			private HttpResponseBase _httpResponse;

			[SetUp]
			public void SetUp()
			{
				_httpResponse = Substitute.For<HttpResponseBase>();

				var inputStream = Substitute.For<Stream>();
				inputStream.CanRead.Returns(false);

				var httpContext = Substitute.For<HttpContextBase>();
				httpContext.Request.HttpMethod.Returns("GET");
				httpContext.Request.Url.Returns(new Uri("https://api.com/exception"));
				httpContext.Request.InputStream.Returns(inputStream);
				httpContext.Response.Returns(_httpResponse);

				PiccoloHttpHandler.ProcessRequest(new PiccoloContext(httpContext));
			}

			[Test]
			public void it_should_raise_request_processing_event()
			{
				_httpResponse.Received().Write("RequestProcessingEvent handled");
			}

			[Test]
			public void it_should_raise_request_processed_event()
			{
				_httpResponse.Received().Write("RequestProcessedEvent handled with StopEventProcessing: ");
			}

			[Test]
			public void it_should_raise_request_faulted_event()
			{
				_httpResponse.Received().Write("RequestFaultedEvent handled+TargetInvocationException");
			}

			[Test]
			public void it_should_return_status_500()
			{
				_httpResponse.Received().StatusCode = (int)HttpStatusCode.InternalServerError;
			}

			[Test]
			public void it_should_return_status_reason_ok()
			{
				_httpResponse.Received().StatusDescription = "Internal Server Error";
			}

			[Test]
			public void it_should_not_return_content()
			{
				_httpResponse.DidNotReceive().Write(Arg.Is<string>(x => !x.Contains("Event")));
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
		public class when_processing_request_with_runtime_exception_in_aspnet_debug_mode : given_http_handler
		{
			private HttpResponseBase _httpResponse;

			[SetUp]
			public void SetUp()
			{
				_httpResponse = Substitute.For<HttpResponseBase>();

				var inputStream = Substitute.For<Stream>();
				inputStream.CanRead.Returns(false);

				var httpContext = Substitute.For<HttpContextBase>();
				httpContext.IsDebuggingEnabled.Returns(true);
				httpContext.Request.HttpMethod.Returns("GET");
				httpContext.Request.Url.Returns(new Uri("https://api.com/exception"));
				httpContext.Request.InputStream.Returns(inputStream);
				httpContext.Response.Returns(_httpResponse);

				PiccoloHttpHandler.ProcessRequest(new PiccoloContext(httpContext));
			}

			[Test]
			public void it_should_raise_request_processing_event()
			{
				_httpResponse.Received().Write("RequestProcessingEvent handled");
			}

			[Test]
			public void it_should_raise_request_processed_event()
			{
				_httpResponse.Received().Write(Arg.Is<string>(x => x.StartsWith("RequestProcessedEvent handled with StopEventProcessing: {\"ClassName\":\"System.Reflection.TargetInvocationException\"")));
			}

			[Test]
			public void it_should_raise_request_faulted_event()
			{
				_httpResponse.Received().Write("RequestFaultedEvent handled+TargetInvocationException");
			}

			[Test]
			public void it_should_return_status_500()
			{
				_httpResponse.Received().StatusCode = (int)HttpStatusCode.InternalServerError;
			}

			[Test]
			public void it_should_return_status_reason_ok()
			{
				_httpResponse.Received().StatusDescription = "Internal Server Error";
			}

			[Test]
			public void it_should_return_exception_information()
			{
				_httpResponse.Received().Write(Arg.Is<string>(x => x.Contains("Exception")));
			}
		}

		[TestFixture]
		public class when_processing_request_with_route_parameter_datatype_mismatch : given_http_handler
		{
			private HttpResponseBase _httpResponse;

			[SetUp]
			public void SetUp()
			{
				_httpResponse = Substitute.For<HttpResponseBase>();

				var inputStream = Substitute.For<Stream>();
				inputStream.CanRead.Returns(false);

				var httpContext = Substitute.For<HttpContextBase>();
				httpContext.Request.HttpMethod.Returns("GET");
				httpContext.Request.Url.Returns(new Uri("https://api.com/route_parameter_datatype_mismatch_exception/test"));
				httpContext.Request.InputStream.Returns(inputStream);
				httpContext.Response.Returns(_httpResponse);

				PiccoloHttpHandler.ProcessRequest(new PiccoloContext(httpContext));
			}

			[Test]
			public void it_should_raise_request_processing_event()
			{
				_httpResponse.Received().Write("RequestProcessingEvent handled");
			}

			[Test]
			public void it_should_raise_request_processed_event()
			{
				_httpResponse.Received().Write("RequestProcessedEvent handled with StopEventProcessing: ");
			}

			[Test]
			public void it_should_raise_request_faulted_event()
			{
				_httpResponse.Received().Write("RequestFaultedEvent handled+RouteParameterDatatypeMismatchException");
			}

			[Test]
			public void it_should_return_status_404()
			{
				_httpResponse.Received().StatusCode = (int)HttpStatusCode.NotFound;
			}

			[Test]
			public void it_should_return_status_reason_not_found()
			{
				_httpResponse.Received().StatusDescription = "Not Found";
			}

			[Test]
			public void it_should_not_return_content()
			{
				_httpResponse.DidNotReceive().Write(Arg.Is<string>(x => !x.Contains("Event")));
			}

			[Route("/route_parameter_datatype_mismatch_exception/{id}")]
			public class HandlerWithRuntimeRouteParameterDatatypeMismatch : IGet<string>
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
		public class when_processing_request_with_route_parameter_datatype_mismatch_in_aspnet_debug_mode : given_http_handler
		{
			private HttpResponseBase _httpResponse;

			[SetUp]
			public void SetUp()
			{
				_httpResponse = Substitute.For<HttpResponseBase>();

				var inputStream = Substitute.For<Stream>();
				inputStream.CanRead.Returns(false);

				var httpContext = Substitute.For<HttpContextBase>();
				httpContext.IsDebuggingEnabled.Returns(true);
				httpContext.Request.HttpMethod.Returns("GET");
				httpContext.Request.Url.Returns(new Uri("https://api.com/route_parameter_datatype_mismatch_exception/test"));
				httpContext.Request.InputStream.Returns(inputStream);
				httpContext.Response.Returns(_httpResponse);

				PiccoloHttpHandler.ProcessRequest(new PiccoloContext(httpContext));
			}

			[Test]
			public void it_should_raise_request_processing_event()
			{
				_httpResponse.Received().Write("RequestProcessingEvent handled");
			}

			[Test]
			public void it_should_raise_request_processed_event()
			{
				_httpResponse.Received().Write(Arg.Is<string>(x => x.StartsWith("RequestProcessedEvent handled with StopEventProcessing: {\"ClassName\":\"Piccolo.Internal.RouteParameterDatatypeMismatchException\"")));
			}

			[Test]
			public void it_should_raise_request_faulted_event()
			{
				_httpResponse.Received().Write("RequestFaultedEvent handled+RouteParameterDatatypeMismatchException");
			}

			[Test]
			public void it_should_return_status_404()
			{
				_httpResponse.Received().StatusCode = (int)HttpStatusCode.NotFound;
			}

			[Test]
			public void it_should_return_status_reason_not_found()
			{
				_httpResponse.Received().StatusDescription = "Not Found";
			}

			[Test]
			public void it_should_return_exception_information()
			{
				_httpResponse.Received().Write(Arg.Is<string>(x => x.Contains("Exception")));
			}
		}

		[TestFixture]
		public class when_processing_request_with_malformed_optional_parameter : given_http_handler
		{
			private HttpResponseBase _httpResponse;

			[SetUp]
			public void SetUp()
			{
				_httpResponse = Substitute.For<HttpResponseBase>();

				var inputStream = Substitute.For<Stream>();
				inputStream.CanRead.Returns(false);

				var httpContext = Substitute.For<HttpContextBase>();
				httpContext.Request.HttpMethod.Returns("GET");
				httpContext.Request.Url.Returns(new Uri("https://api.com/malformed_optional_parameter?id=test"));
				httpContext.Request.InputStream.Returns(inputStream);
				httpContext.Response.Returns(_httpResponse);

				PiccoloHttpHandler.ProcessRequest(new PiccoloContext(httpContext));
			}

			[Test]
			public void it_should_raise_request_processing_event()
			{
				_httpResponse.Received().Write("RequestProcessingEvent handled");
			}

			[Test]
			public void it_should_raise_request_processed_event()
			{
				_httpResponse.Received().Write("RequestProcessedEvent handled with StopEventProcessing: ");
			}

			[Test]
			public void it_should_raise_request_faulted_event()
			{
				_httpResponse.Received().Write("RequestFaultedEvent handled+MalformedParameterException");
			}

			[Test]
			public void it_should_return_status_400()
			{
				_httpResponse.Received().StatusCode = (int)HttpStatusCode.BadRequest;
			}

			[Test]
			public void it_should_return_status_reason_not_found()
			{
				_httpResponse.Received().StatusDescription = "Bad Request";
			}

			[Test]
			public void it_should_not_return_content()
			{
				_httpResponse.DidNotReceive().Write(Arg.Is<string>(x => !x.Contains("Event")));
			}

			[Route("/malformed_optional_parameter")]
			public class HandlerWithMalformedOptionalParameter : IGet<string>
			{
				[Optional]
				public int Id { get; set; }

				[ExcludeFromCodeCoverage]
				public HttpResponseMessage<string> Get()
				{
					return Response.Success.NoContent<string>();
				}
			}
		}

		[TestFixture]
		public class when_processing_request_with_malformed_optional_parameter_in_aspnet_debug_mode : given_http_handler
		{
			private HttpResponseBase _httpResponse;

			[SetUp]
			public void SetUp()
			{
				_httpResponse = Substitute.For<HttpResponseBase>();

				var inputStream = Substitute.For<Stream>();
				inputStream.CanRead.Returns(false);

				var httpContext = Substitute.For<HttpContextBase>();
				httpContext.IsDebuggingEnabled.Returns(true);
				httpContext.Request.HttpMethod.Returns("GET");
				httpContext.Request.Url.Returns(new Uri("https://api.com/malformed_optional_parameter?id=test"));
				httpContext.Request.InputStream.Returns(inputStream);
				httpContext.Response.Returns(_httpResponse);

				PiccoloHttpHandler.ProcessRequest(new PiccoloContext(httpContext));
			}

			[Test]
			public void it_should_raise_request_processing_event()
			{
				_httpResponse.Received().Write("RequestProcessingEvent handled");
			}

			[Test]
			public void it_should_raise_request_processed_event()
			{
				_httpResponse.Received().Write(Arg.Is<string>(x => x.StartsWith("RequestProcessedEvent handled with StopEventProcessing: {\"ClassName\":\"Piccolo.Internal.MalformedParameterException\"")));
			}

			[Test]
			public void it_should_raise_request_faulted_event()
			{
				_httpResponse.Received().Write("RequestFaultedEvent handled+MalformedParameterException");
			}

			[Test]
			public void it_should_return_status_400()
			{
				_httpResponse.Received().StatusCode = (int)HttpStatusCode.BadRequest;
			}

			[Test]
			public void it_should_return_status_reason_not_found()
			{
				_httpResponse.Received().StatusDescription = "Bad Request";
			}

			[Test]
			public void it_should_return_exception_information()
			{
				_httpResponse.Received().Write(Arg.Is<string>(x => x.Contains("Exception")));
			}
		}

		[TestFixture]
		public class when_processing_request_with_malformed_payload : given_http_handler
		{
			private HttpResponseBase _httpResponse;

			[SetUp]
			public void SetUp()
			{
				_httpResponse = Substitute.For<HttpResponseBase>();

				var httpContext = Substitute.For<HttpContextBase>();
				httpContext.Request.HttpMethod.Returns("POST");
				httpContext.Request.Url.Returns(new Uri("https://api.com/malformed_payload_exception"));
				httpContext.Request.InputStream.Returns(new MemoryStream(Encoding.UTF8.GetBytes("invalid")));
				httpContext.Response.Returns(_httpResponse);

				PiccoloHttpHandler.ProcessRequest(new PiccoloContext(httpContext));
			}

			[Test]
			public void it_should_raise_request_processing_event()
			{
				_httpResponse.Received().Write("RequestProcessingEvent handled");
			}

			[Test]
			public void it_should_raise_request_processed_event()
			{
				_httpResponse.Received().Write("RequestProcessedEvent handled with StopEventProcessing: ");
			}

			[Test]
			public void it_should_raise_request_faulted_event()
			{
				_httpResponse.Received().Write("RequestFaultedEvent handled+MalformedPayloadException");
			}

			[Test]
			public void it_should_return_status_422()
			{
				_httpResponse.Received().StatusCode = 422;
			}

			[Test]
			public void it_should_return_status_reason_unprocessable_entity()
			{
				_httpResponse.Received().StatusDescription = "Unprocessable Entity";
			}

			[Test]
			public void it_should_not_return_content()
			{
				_httpResponse.DidNotReceive().Write(Arg.Is<string>(x => !x.Contains("Event")));
			}

			[Route("/malformed_payload_exception")]
			public class HandlerWithRuntimeRouteParameterDatatypeMismatch : IPost<DateTime, string>
			{
				[ExcludeFromCodeCoverage]
				public HttpResponseMessage<string> Post(DateTime parameters)
				{
					return Response.Success.NoContent<string>();
				}
			}
		}

		[TestFixture]
		public class when_processing_request_with_alternate_malformed_payload : given_http_handler
		{
			private HttpResponseBase _httpResponse;

			[SetUp]
			public void SetUp()
			{
				_httpResponse = Substitute.For<HttpResponseBase>();

				var httpContext = Substitute.For<HttpContextBase>();
				httpContext.Request.HttpMethod.Returns("POST");
				httpContext.Request.Url.Returns(new Uri("https://api.com/alternate_malformed_payload_exception"));
				httpContext.Request.InputStream.Returns(new MemoryStream(Encoding.UTF8.GetBytes("{ error: \"\" ")));
				httpContext.Response.Returns(_httpResponse);

				PiccoloHttpHandler.ProcessRequest(new PiccoloContext(httpContext));
			}

			[Test]
			public void it_should_raise_request_processing_event()
			{
				_httpResponse.Received().Write("RequestProcessingEvent handled");
			}

			[Test]
			public void it_should_raise_request_processed_event()
			{
				_httpResponse.Received().Write("RequestProcessedEvent handled with StopEventProcessing: ");
			}

			[Test]
			public void it_should_raise_request_faulted_event()
			{
				_httpResponse.Received().Write("RequestFaultedEvent handled+MalformedPayloadException");
			}

			[Test]
			public void it_should_return_status_422()
			{
				_httpResponse.Received().StatusCode = 422;
			}

			[Test]
			public void it_should_return_status_reason_unprocessable_entity()
			{
				_httpResponse.Received().StatusDescription = "Unprocessable Entity";
			}

			[Test]
			public void it_should_not_return_content()
			{
				_httpResponse.DidNotReceive().Write(Arg.Is<string>(x => !x.Contains("Event")));
			}

			[Route("/alternate_malformed_payload_exception")]
			public class HandlerWithRuntimeRouteParameterDatatypeMismatch : IPost<Params, string>
			{
				[ExcludeFromCodeCoverage]
				public HttpResponseMessage<string> Post(Params parameters)
				{
					return Response.Success.NoContent<string>();
				}
			}

			public class Params
			{
				public string Message { get; set; }
			}
		}

		[TestFixture]
		public class when_processing_request_with_malformed_payload_in_aspnet_debug_mode : given_http_handler
		{
			private HttpResponseBase _httpResponse;

			[SetUp]
			public void SetUp()
			{
				_httpResponse = Substitute.For<HttpResponseBase>();

				var httpContext = Substitute.For<HttpContextBase>();
				httpContext.IsDebuggingEnabled.Returns(true);
				httpContext.Request.HttpMethod.Returns("POST");
				httpContext.Request.Url.Returns(new Uri("https://api.com/malformed_payload_exception"));
				httpContext.Request.InputStream.Returns(new MemoryStream(Encoding.UTF8.GetBytes("invalid")));
				httpContext.Response.Returns(_httpResponse);

				PiccoloHttpHandler.ProcessRequest(new PiccoloContext(httpContext));
			}

			[Test]
			public void it_should_raise_request_processing_event()
			{
				_httpResponse.Received().Write("RequestProcessingEvent handled");
			}

			[Test]
			public void it_should_raise_request_processed_event()
			{
				_httpResponse.Received().Write(Arg.Is<string>(x => x.StartsWith("RequestProcessedEvent handled with StopEventProcessing: {\"ClassName\":\"Piccolo.Internal.MalformedPayloadException\"")));
			}

			[Test]
			public void it_should_raise_request_faulted_event()
			{
				_httpResponse.Received().Write("RequestFaultedEvent handled+MalformedPayloadException");
			}

			[Test]
			public void it_should_return_status_422()
			{
				_httpResponse.Received().StatusCode = 422;
			}

			[Test]
			public void it_should_return_status_reason_unprocessable_entity()
			{
				_httpResponse.Received().StatusDescription = "Unprocessable Entity";
			}

			[Test]
			public void it_should_return_exception_information()
			{
				_httpResponse.Received().Write(Arg.Is<string>(s => s.Contains("Exception")));
			}
		}

		[TestFixture]
		public class when_processing_POST_request_with_missing_payload : given_http_handler
		{
			private HttpResponseBase _httpResponse;

			[SetUp]
			public void SetUp()
			{
				_httpResponse = Substitute.For<HttpResponseBase>();

				var inputStream = Substitute.For<Stream>();
				inputStream.CanRead.Returns(false);

				var httpContext = Substitute.For<HttpContextBase>();
				httpContext.Request.HttpMethod.Returns("POST");
				httpContext.Request.Url.Returns(new Uri("https://api.com/test-resources"));
				httpContext.Request.InputStream.Returns(inputStream);
				httpContext.Response.Returns(_httpResponse);

				PiccoloHttpHandler.ProcessRequest(new PiccoloContext(httpContext));
			}

			[Test]
			public void it_should_raise_request_processing_event()
			{
				_httpResponse.Received().Write("RequestProcessingEvent handled");
			}

			[Test]
			public void it_should_raise_request_processed_event()
			{
				_httpResponse.Received().Write("RequestProcessedEvent handled with StopEventProcessing: {\"error\":\"Payload missing\"}");
			}

			[Test]
			public void it_should_raise_request_faulted_event()
			{
				_httpResponse.Received().Write("RequestFaultedEvent handled+MissingPayloadException");
			}

			[Test]
			public void it_should_return_status_400()
			{
				_httpResponse.Received().StatusCode = 400;
			}

			[Test]
			public void it_should_return_status_reason_unprocessable_entity()
			{
				_httpResponse.Received().StatusDescription = "Bad Request";
			}

			[Test]
			public void it_should_return_error_information()
			{
				_httpResponse.Received().Write(Arg.Is<string>(x => x.Contains("Payload missing")));
			}
		}

		[TestFixture]
		public class when_processing_PUT_request_with_missing_payload : given_http_handler
		{
			private HttpResponseBase _httpResponse;

			[SetUp]
			public void SetUp()
			{
				_httpResponse = Substitute.For<HttpResponseBase>();

				var inputStream = Substitute.For<Stream>();
				inputStream.CanRead.Returns(false);

				var httpContext = Substitute.For<HttpContextBase>();
				httpContext.Request.HttpMethod.Returns("PUT");
				httpContext.Request.Url.Returns(new Uri("https://api.com/test-resources/1"));
				httpContext.Request.InputStream.Returns(inputStream);
				httpContext.Response.Returns(_httpResponse);

				PiccoloHttpHandler.ProcessRequest(new PiccoloContext(httpContext));
			}

			[Test]
			public void it_should_raise_request_processing_event()
			{
				_httpResponse.Received().Write("RequestProcessingEvent handled");
			}

			[Test]
			public void it_should_raise_request_processed_event()
			{
				_httpResponse.Received().Write("RequestProcessedEvent handled with StopEventProcessing: {\"error\":\"Payload missing\"}");
			}

			[Test]
			public void it_should_raise_request_faulted_event()
			{
				_httpResponse.Received().Write("RequestFaultedEvent handled+MissingPayloadException");
			}

			[Test]
			public void it_should_return_status_400()
			{
				_httpResponse.Received().StatusCode = 400;
			}

			[Test]
			public void it_should_return_status_reason_unprocessable_entity()
			{
				_httpResponse.Received().StatusDescription = "Bad Request";
			}

			[Test]
			public void it_should_return_error_information()
			{
				_httpResponse.Received().Write(Arg.Is<string>(x => x.Contains("Payload missing")));
			}
		}

		#endregion

		#region Validation

		[TestFixture]
		public class when_processing_request_with_invalid_payload : given_http_handler
		{
			private HttpResponseBase _httpResponse;

			[SetUp]
			public void SetUp()
			{
				_httpResponse = Substitute.For<HttpResponseBase>();

				var httpContext = Substitute.For<HttpContextBase>();
				httpContext.Request.HttpMethod.Returns("POST");
				httpContext.Request.Url.Returns(new Uri("https://api.com/validation/payload"));
				httpContext.Request.InputStream.Returns(new MemoryStream(Encoding.UTF8.GetBytes("{\"A\":\"\"}")));
				httpContext.Response.Returns(_httpResponse);

				var piccoloContext = new PiccoloContext(httpContext);

				PiccoloHttpHandler.ProcessRequest(piccoloContext);
			}

			[Test]
			public void it_should_raise_request_processing_event()
			{
				_httpResponse.Received().Write("RequestProcessingEvent handled");
			}

			[Test]
			public void it_should_raise_request_processed_event()
			{
				_httpResponse.Received().Write("RequestProcessedEvent handled with StopEventProcessing: {\"error\":\"invalid\"}");
			}

			[Test]
			public void it_should_not_raise_request_faulted_event()
			{
				_httpResponse.DidNotReceive().Write("RequestFaultedEvent handled");
			}

			[Test]
			public void it_should_return_status_400()
			{
				_httpResponse.Received().StatusCode = 400;
			}

			[Test]
			public void it_should_return_error_message()
			{
				_httpResponse.Received().Write("{\"error\":\"invalid\"}");
			}
		}

		[TestFixture]
		public class when_processing_request_with_invalid_optional_parameter : given_http_handler
		{
			private HttpResponseBase _httpResponse;

			[SetUp]
			public void SetUp()
			{
				_httpResponse = Substitute.For<HttpResponseBase>();

				var inputStream = Substitute.For<Stream>();
				inputStream.CanRead.Returns(false);

				var httpContext = Substitute.For<HttpContextBase>();
				httpContext.Request.HttpMethod.Returns("GET");
				httpContext.Request.Url.Returns(new Uri("https://api.com/validation/optional-parameter?age=-1"));
				httpContext.Request.InputStream.Returns(inputStream);
				httpContext.Response.Returns(_httpResponse);

				var piccoloContext = new PiccoloContext(httpContext);

				PiccoloHttpHandler.ProcessRequest(piccoloContext);
			}

			[Test]
			public void it_should_raise_request_processing_event()
			{
				_httpResponse.Received().Write("RequestProcessingEvent handled");
			}

			[Test]
			public void it_should_raise_request_processed_event()
			{
				_httpResponse.Received().Write("RequestProcessedEvent handled with StopEventProcessing: {\"error\":\"invalid age\"}");
			}

			[Test]
			public void it_should_not_raise_request_faulted_event()
			{
				_httpResponse.DidNotReceive().Write("RequestFaultedEvent handled");
			}

			[Test]
			public void it_should_return_status_400()
			{
				_httpResponse.Received().StatusCode = 400;
			}

			[Test]
			public void it_should_return_error_message()
			{
				_httpResponse.Received().Write("{\"error\":\"invalid age\"}");
			}
		}

		[Route("/validation/payload")]
		[ValidateWith(typeof(TestPayloadValidator))]
		public class RequestHandlerWithPayloadValidator : IPost<RequestHandlerInvokerTests.TestResourceWithPayload.Parameters, string>
		{
			[ExcludeFromCodeCoverage]
			public HttpResponseMessage<string> Post(RequestHandlerInvokerTests.TestResourceWithPayload.Parameters parameters)
			{
				return Response.Success.Ok(string.Empty);
			}
		}

		[Route("/validation/optional-parameter")]
		[ExcludeFromCodeCoverage]
		public class RequestHandlerWithParameterValidator : IGet<string>
		{
			[Optional]
			[ValidateWith(typeof(TestParameterValidator))]
			public int Age { get; set; }

			public HttpResponseMessage<string> Get()
			{
				return Response.Success.Ok(string.Empty);
			}
		}

		#endregion

		[TestFixture]
		public class when_processing_get_request_cancelled_by_event_handler
		{
			private HttpResponseBase _httpResponse;

			[SetUp]
			public void SetUp()
			{
				_httpResponse = Substitute.For<HttpResponseBase>();

				var inputStream = Substitute.For<Stream>();
				inputStream.CanRead.Returns(false);

				var httpContext = Substitute.For<HttpContextBase>();
				httpContext.Request.HttpMethod.Returns("GET");
				httpContext.Request.Url.Returns(new Uri("https://api.com/test-resources/1"));
				httpContext.Request.InputStream.Returns(inputStream);
				httpContext.Response.Returns(_httpResponse);

				var piccoloContext = new PiccoloContext(httpContext);

				var piccoloConfiguration = Bootstrapper.ApplyConfiguration(Assembly.GetExecutingAssembly(), false);
				piccoloConfiguration.EventHandlers.RequestProcessing.Remove<EventDispatcherTests.TestRequestProcessingEventHandlerWithStopEventProcessing>();

				var httpHandler = new PiccoloHttpHandler(piccoloConfiguration);

				httpHandler.ProcessRequest(piccoloContext);
			}

			[Test]
			public void it_should_raise_request_processing_event()
			{
				_httpResponse.Received().Write("RequestProcessingEvent handled");
			}

			[Test]
			public void it_should_raise_request_processed_event()
			{
				_httpResponse.Received().Write("RequestProcessedEvent handled with StopEventProcessing: ");
			}

			[Test]
			public void it_should_not_raise_request_faulted_event()
			{
				_httpResponse.DidNotReceive().Write("RequestFaultedEvent handled");
			}

			[Test]
			public void it_should_return_status_426()
			{
				_httpResponse.Received().StatusCode = 426;
			}

			[Test]
			public void it_should_not_return_content()
			{
				_httpResponse.DidNotReceive().Write(Arg.Is<string>(x => !x.Contains("Event")));
			}
		}

		[TestFixture]
		public class when_processing_request_with_unsupported_verb : given_http_handler
		{
			private HttpResponseBase _httpResponse;

			[SetUp]
			public void SetUp()
			{
				_httpResponse = Substitute.For<HttpResponseBase>();

				var inputStream = Substitute.For<Stream>();
				inputStream.CanRead.Returns(false);

				var httpContext = Substitute.For<HttpContextBase>();
				httpContext.Request.HttpMethod.Returns("FAKE!");
				httpContext.Request.Url.Returns(new Uri("https://api.com/"));
				httpContext.Request.InputStream.Returns(inputStream);
				httpContext.Response.Returns(_httpResponse);

				PiccoloHttpHandler.ProcessRequest(new PiccoloContext(httpContext));
			}

			[Test]
			public void it_should_raise_request_processing_event()
			{
				_httpResponse.Received().Write("RequestProcessingEvent handled");
			}

			[Test]
			public void it_should_raise_request_processed_event()
			{
				_httpResponse.Received().Write("RequestProcessedEvent handled with StopEventProcessing: ");
			}

			[Test]
			public void it_should_not_raise_request_faulted_event()
			{
				_httpResponse.DidNotReceive().Write("RequestFaultedEvent handled");
			}

			[Test]
			public void it_should_return_status_404()
			{
				_httpResponse.Received().StatusCode = (int)HttpStatusCode.NotFound;
			}

			[Test]
			public void it_should_return_status_reason_method_not_allowed()
			{
				_httpResponse.Received().StatusDescription = "Not Found";
			}

			[Test]
			public void it_should_not_return_content()
			{
				_httpResponse.DidNotReceive().Write(Arg.Is<string>(x => !x.Contains("Event")));
			}
		}

		[TestFixture]
		public class when_processing_request_to_unhandled_resource : given_http_handler
		{
			private HttpResponseBase _httpResponse;

			[SetUp]
			public void SetUp()
			{
				_httpResponse = Substitute.For<HttpResponseBase>();

				var inputStream = Substitute.For<Stream>();
				inputStream.CanRead.Returns(false);

				var httpContext = Substitute.For<HttpContextBase>();
				httpContext.Request.HttpMethod.Returns("GET");
				httpContext.Request.Url.Returns(new Uri("https://api.com/unhandled/resource"));
				httpContext.Request.InputStream.Returns(inputStream);
				httpContext.Response.Returns(_httpResponse);

				PiccoloHttpHandler.ProcessRequest(new PiccoloContext(httpContext));
			}

			[Test]
			public void it_should_raise_request_processing_event()
			{
				_httpResponse.Received().Write("RequestProcessingEvent handled");
			}

			[Test]
			public void it_should_raise_request_processed_event()
			{
				_httpResponse.Received().Write("RequestProcessedEvent handled with StopEventProcessing: ");
			}

			[Test]
			public void it_should_not_raise_request_faulted_event()
			{
				_httpResponse.DidNotReceive().Write("RequestFaultedEvent handled");
			}

			[Test]
			public void it_should_return_status_404()
			{
				_httpResponse.Received().StatusCode = (int)HttpStatusCode.NotFound;
			}

			[Test]
			public void it_should_return_status_reason_not_found()
			{
				_httpResponse.Received().StatusDescription = "Not Found";
			}

			[Test]
			public void it_should_not_return_content()
			{
				_httpResponse.DidNotReceive().Write(Arg.Is<string>(x => !x.Contains("Event")));
			}
		}

		public abstract class given_http_handler
		{
			protected PiccoloHttpHandler PiccoloHttpHandler;

			protected given_http_handler()
			{
				var piccoloConfiguration = Bootstrapper.ApplyConfiguration(Assembly.GetExecutingAssembly(), false);
				piccoloConfiguration.EventHandlers.RequestProcessing.Remove<EventDispatcherTests.TestRequestProcessingEventHandlerWithStopEventProcessing>();
				piccoloConfiguration.EventHandlers.RequestProcessing.Remove<EventDispatcherTests.TestRequestProcessingEventHandlerWithStopRequestProcessing>();

				PiccoloHttpHandler = new PiccoloHttpHandler(piccoloConfiguration);
			}
		}
	}
}