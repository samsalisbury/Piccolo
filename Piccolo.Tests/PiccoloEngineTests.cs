﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Web;
using NSubstitute;
using NUnit.Framework;
using Piccolo.Configuration;
using Piccolo.Events;
using Piccolo.Request;
using Piccolo.Routing;
using Piccolo.Validation;
using Shouldly;

namespace Piccolo.Tests
{
	public class PiccoloEngineTests
	{
		[TestFixture]
		public class when_processing_request : given_piccolo_engine
		{
			private PiccoloContext _piccoloContext;

			[SetUp]
			public void SetUp()
			{
				const string verb = "GET";
				const string applicationPath = "/";
				var uri = new Uri("http://example.com/resources/1?query=2");
				var routeParameters = new Dictionary<string, string> {{"route", "1"}};

				HttpContextBase.Request.HttpMethod.Returns(verb);
				HttpContextBase.Request.ApplicationPath.Returns(applicationPath);
				HttpContextBase.Request.Url.Returns(uri);

				var inputStream = Substitute.For<Stream>();
				inputStream.CanRead.Returns(false);
				HttpContextBase.Request.InputStream.Returns(inputStream);

				RequestRouter.FindRequestHandler(verb, applicationPath, uri).Returns(new RouteHandlerLookupResult(typeof(GetResource), routeParameters));

				RequestHandlerInvoker.Execute(
					Arg.Any<GetResource>(),
					verb,
					routeParameters,
					Arg.Is<IDictionary<string, string>>(x => x["query"] == "2"),
					Arg.Is<IDictionary<string, object>>(x => (string)x["context"] == "3"),
					Arg.Is<string>(x => x == null)).Returns(new HttpResponseMessage {Content = new ObjectContent("test")});

				_piccoloContext = new PiccoloContext(HttpContextBase);
				_piccoloContext.Data.context = "3";

				Engine.ProcessRequest(_piccoloContext);
			}

			[Test]
			public void it_should_raise_request_processing_event()
			{
				EventDispatcher.Received().RaiseRequestProcessingEvent(_piccoloContext);
			}

			[Test]
			public void it_should_not_raise_request_faulted_event()
			{
				EventDispatcher.DidNotReceive().RaiseRequestFaultedEvent(_piccoloContext, Arg.Any<Exception>());
			}

			[Test]
			public void it_should_raise_request_processed_event()
			{
				EventDispatcher.Received().RaiseRequestProcessedEvent(_piccoloContext, "\"test\"");
			}

			[Test]
			public void it_should_return_status_code_200()
			{
				HttpContextBase.Response.StatusCode.Returns(200);
			}

			[Test]
			public void it_should_return_status_description_ok()
			{
				HttpContextBase.Response.StatusDescription.Returns("OK");
			}

			[Test]
			public void it_should_set_content_type_header_application_json()
			{
				HttpContextBase.Response.ContentType.ShouldBe("application/json");
			}

			[Test]
			public void it_should_return_response_payload()
			{
				HttpContextBase.Response.Received().Write("\"test\"");
			}

			[ExcludeFromCodeCoverage]
			public class GetResource : IGet<string>
			{
				public HttpResponseMessage<string> Get()
				{
					return null;
				}
			}
		}

		[TestFixture]
		public class when_processing_request_that_does_not_return_response_payload : given_piccolo_engine
		{
			private PiccoloContext _piccoloContext;

			[SetUp]
			public void SetUp()
			{
				const string verb = "DELETE";
				const string applicationPath = "/";
				var uri = new Uri("http://example.com/resources/1");
				var routeParameters = new Dictionary<string, string> {{"route", "1"}};

				HttpContextBase.Request.HttpMethod.Returns(verb);
				HttpContextBase.Request.ApplicationPath.Returns(applicationPath);
				HttpContextBase.Request.Url.Returns(uri);

				var inputStream = Substitute.For<Stream>();
				inputStream.CanRead.Returns(false);
				HttpContextBase.Request.InputStream.Returns(inputStream);

				RequestRouter.FindRequestHandler(verb, applicationPath, uri).Returns(new RouteHandlerLookupResult(typeof(DeleteResource), routeParameters));

				RequestHandlerInvoker.Execute(
					Arg.Any<DeleteResource>(),
					verb,
					routeParameters,
					Arg.Is<IDictionary<string, string>>(x => x.Count == 0),
					Arg.Any<IDictionary<string, object>>(),
					Arg.Is<string>(x => x == string.Empty)).Returns(new HttpResponseMessage(HttpStatusCode.NoContent));

				_piccoloContext = new PiccoloContext(HttpContextBase);

				Engine.ProcessRequest(_piccoloContext);
			}

			[Test]
			public void it_should_return_status_code_204()
			{
				HttpContextBase.Response.StatusCode.Returns(204);
			}

			[Test]
			public void it_should_return_status_description_no_content()
			{
				HttpContextBase.Response.StatusDescription.Returns("No Content");
			}

			[Test]
			public void it_should_not_return_response_payload()
			{
				HttpContextBase.Response.DidNotReceive().Write(Arg.Any<string>());
			}

			[ExcludeFromCodeCoverage]
			public class DeleteResource : IGet<string>
			{
				public HttpResponseMessage<string> Get()
				{
					return null;
				}
			}
		}

		[TestFixture]
		public class when_processing_request_that_sets_location_header : given_piccolo_engine
		{
			private PiccoloContext _piccoloContext;

			[SetUp]
			public void SetUp()
			{
				const string verb = "POST";
				const string applicationPath = "/";
				var uri = new Uri("http://example.com/resources");
				var routeParameters = new Dictionary<string, string>();

				HttpContextBase.Request.HttpMethod.Returns(verb);
				HttpContextBase.Request.ApplicationPath.Returns(applicationPath);
				HttpContextBase.Request.Url.Returns(uri);
				HttpContextBase.Request.InputStream.Returns(new MemoryStream(Encoding.UTF8.GetBytes("\"request_payload\"")));

				RequestRouter.FindRequestHandler(verb, applicationPath, uri).Returns(new RouteHandlerLookupResult(typeof(CreateResource), routeParameters));

				var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.Created);
				httpResponseMessage.Headers.Location = new Uri("http://example.com/resources/1");

				RequestHandlerInvoker.Execute(
					Arg.Any<CreateResource>(),
					verb,
					routeParameters,
					Arg.Is<IDictionary<string, string>>(x => x.Count == 0),
					Arg.Any<IDictionary<string, object>>(),
					Arg.Is<string>(x => x == "request_payload")).Returns(httpResponseMessage);

				_piccoloContext = new PiccoloContext(HttpContextBase);

				Engine.ProcessRequest(_piccoloContext);
			}

			[Test]
			public void it_should_set_location_response_header()
			{
				HttpContextBase.Response.Received().AddHeader("Location", "http://example.com/resources/1");
			}

			[ExcludeFromCodeCoverage]
			public class CreateResource : IPost<string, string>
			{
				public HttpResponseMessage<string> Post(string parameters)
				{
					return null;
				}
			}
		}

		[TestFixture]
		public class when_processing_request_that_returns_a_404_result : given_piccolo_engine
		{
			private PiccoloContext _piccoloContext;

			[SetUp]
			public void SetUp()
			{
				const string verb = "POST";
				const string applicationPath = "/";
				var uri = new Uri("http://example.com/resources/1");

				HttpContextBase.Request.HttpMethod.Returns(verb);
				HttpContextBase.Request.ApplicationPath.Returns(applicationPath);
				HttpContextBase.Request.Url.Returns(uri);

				var inputStream = Substitute.For<Stream>();
				inputStream.CanRead.Returns(false);
				HttpContextBase.Request.InputStream.Returns(inputStream);

				RequestRouter.FindRequestHandler(verb, applicationPath, uri).Returns(RouteHandlerLookupResult.FailedResult);

				_piccoloContext = new PiccoloContext(HttpContextBase);

				Engine.ProcessRequest(_piccoloContext);
			}

			[Test]
			public void it_should_return_status_code_404()
			{
				HttpContextBase.Response.StatusCode.Returns(404);
			}

			[Test]
			public void it_should_return_status_description_not_found()
			{
				HttpContextBase.Response.StatusDescription.Returns("Not Found");
			}
		}

		[TestFixture]
		public class when_processing_request_that_sends_an_invalid_route_parameter_type_and_http_debugging_is_disabled : given_piccolo_engine
		{
			private PiccoloContext _piccoloContext;

			[SetUp]
			public void SetUp()
			{
				const string verb = "POST";
				const string applicationPath = "/";
				var uri = new Uri("http://example.com/resources/1");

				HttpContextBase.Request.HttpMethod.Returns(verb);
				HttpContextBase.Request.ApplicationPath.Returns(applicationPath);
				HttpContextBase.Request.Url.Returns(uri);

				var inputStream = Substitute.For<Stream>();
				inputStream.CanRead.Returns(false);
				HttpContextBase.Request.InputStream.Returns(inputStream);

				RequestRouter.When(x => x.FindRequestHandler(verb, applicationPath, uri)).Do(_ => { throw new RouteParameterDatatypeMismatchException(); });

				_piccoloContext = new PiccoloContext(HttpContextBase);

				Engine.ProcessRequest(_piccoloContext);
			}

			[Test]
			public void it_should_raise_request_faulted_event()
			{
				EventDispatcher.Received().RaiseRequestFaultedEvent(_piccoloContext, Arg.Any<RouteParameterDatatypeMismatchException>());
			}

			[Test]
			public void it_should_return_status_code_404()
			{
				HttpContextBase.Response.StatusCode.Returns(404);
			}

			[Test]
			public void it_should_return_status_description_not_found()
			{
				HttpContextBase.Response.StatusDescription.Returns("Not Found");
			}

			[Test]
			public void it_should_not_return_response_payload()
			{
				HttpContextBase.Response.DidNotReceive().Write(Arg.Any<string>());
			}
		}

		[TestFixture]
		public class when_processing_request_that_sends_an_invalid_route_parameter_type_and_http_debugging_is_enabled : given_piccolo_engine
		{
			private PiccoloContext _piccoloContext;

			[SetUp]
			public void SetUp()
			{
				const string verb = "POST";
				const string applicationPath = "/";
				var uri = new Uri("http://example.com/resources/1");

				HttpContextBase.Request.HttpMethod.Returns(verb);
				HttpContextBase.Request.ApplicationPath.Returns(applicationPath);
				HttpContextBase.Request.Url.Returns(uri);
				HttpContextBase.IsDebuggingEnabled.Returns(true);

				var inputStream = Substitute.For<Stream>();
				inputStream.CanRead.Returns(false);
				HttpContextBase.Request.InputStream.Returns(inputStream);

				RequestRouter.When(x => x.FindRequestHandler(verb, applicationPath, uri)).Do(_ => { throw new RouteParameterDatatypeMismatchException(); });

				_piccoloContext = new PiccoloContext(HttpContextBase);

				Engine.ProcessRequest(_piccoloContext);
			}

			[Test]
			public void it_should_return_response_payload()
			{
				HttpContextBase.Response.Received().Write(Arg.Is<string>(x => x.Contains("RouteParameterDatatypeMismatchException")));
			}
		}

		[TestFixture]
		public class when_processing_request_that_sends_a_malformed_parameter_type_and_http_debugging_is_disabled : given_piccolo_engine
		{
			private PiccoloContext _piccoloContext;

			[SetUp]
			public void SetUp()
			{
				const string verb = "POST";
				const string applicationPath = "/";
				var uri = new Uri("http://example.com/resources/1");

				HttpContextBase.Request.HttpMethod.Returns(verb);
				HttpContextBase.Request.ApplicationPath.Returns(applicationPath);
				HttpContextBase.Request.Url.Returns(uri);

				var inputStream = Substitute.For<Stream>();
				inputStream.CanRead.Returns(false);
				HttpContextBase.Request.InputStream.Returns(inputStream);

				RequestRouter.When(x => x.FindRequestHandler(verb, applicationPath, uri)).Do(_ => { throw new MalformedParameterException(); });

				_piccoloContext = new PiccoloContext(HttpContextBase);

				Engine.ProcessRequest(_piccoloContext);
			}

			[Test]
			public void it_should_raise_request_faulted_event()
			{
				EventDispatcher.Received().RaiseRequestFaultedEvent(_piccoloContext, Arg.Any<MalformedParameterException>());
			}

			[Test]
			public void it_should_return_status_code_400()
			{
				HttpContextBase.Response.StatusCode.Returns(400);
			}

			[Test]
			public void it_should_return_status_description_bad_request()
			{
				HttpContextBase.Response.StatusDescription.Returns("Bad Request");
			}

			[Test]
			public void it_should_not_return_response_payload()
			{
				HttpContextBase.Response.DidNotReceive().Write(Arg.Any<string>());
			}
		}

		[TestFixture]
		public class when_processing_request_that_sends_a_malformed_parameter_type_and_http_debugging_is_enabled : given_piccolo_engine
		{
			private PiccoloContext _piccoloContext;

			[SetUp]
			public void SetUp()
			{
				const string verb = "POST";
				const string applicationPath = "/";
				var uri = new Uri("http://example.com/resources/1");

				HttpContextBase.Request.HttpMethod.Returns(verb);
				HttpContextBase.Request.ApplicationPath.Returns(applicationPath);
				HttpContextBase.Request.Url.Returns(uri);
				HttpContextBase.IsDebuggingEnabled.Returns(true);

				var inputStream = Substitute.For<Stream>();
				inputStream.CanRead.Returns(false);
				HttpContextBase.Request.InputStream.Returns(inputStream);

				RequestRouter.When(x => x.FindRequestHandler(verb, applicationPath, uri)).Do(_ => { throw new MalformedParameterException(); });

				_piccoloContext = new PiccoloContext(HttpContextBase);

				Engine.ProcessRequest(_piccoloContext);
			}

			[Test]
			public void it_should_return_response_payload()
			{
				HttpContextBase.Response.Received().Write(Arg.Is<string>(x => x.Contains("MalformedParameterException")));
			}
		}

		[TestFixture]
		public class when_processing_request_that_is_missing_payload_and_http_debugging_is_disabled : given_piccolo_engine
		{
			private PiccoloContext _piccoloContext;

			[SetUp]
			public void SetUp()
			{
				const string verb = "POST";
				const string applicationPath = "/";
				var uri = new Uri("http://example.com/resources/1");

				HttpContextBase.Request.HttpMethod.Returns(verb);
				HttpContextBase.Request.ApplicationPath.Returns(applicationPath);
				HttpContextBase.Request.Url.Returns(uri);

				var inputStream = Substitute.For<Stream>();
				inputStream.CanRead.Returns(false);
				HttpContextBase.Request.InputStream.Returns(inputStream);

				RequestRouter.When(x => x.FindRequestHandler(verb, applicationPath, uri)).Do(_ => { throw new MissingPayloadException(); });

				_piccoloContext = new PiccoloContext(HttpContextBase);

				Engine.ProcessRequest(_piccoloContext);
			}

			[Test]
			public void it_should_raise_request_faulted_event()
			{
				EventDispatcher.Received().RaiseRequestFaultedEvent(_piccoloContext, Arg.Any<MissingPayloadException>());
			}

			[Test]
			public void it_should_return_status_code_400()
			{
				HttpContextBase.Response.StatusCode.Returns(400);
			}

			[Test]
			public void it_should_return_status_description_bad_request()
			{
				HttpContextBase.Response.StatusDescription.Returns("Bad Request");
			}

			[Test]
			public void it_should_return_response_payload()
			{
				HttpContextBase.Response.Received().Write(Arg.Is<string>(x => x.Contains("Payload missing")));
			}
		}

		[TestFixture]
		public class when_processing_request_that_sends_a_malformed_payload_and_http_debugging_is_disabled : given_piccolo_engine
		{
			private PiccoloContext _piccoloContext;

			[SetUp]
			public void SetUp()
			{
				const string verb = "POST";
				const string applicationPath = "/";
				var uri = new Uri("http://example.com/resources/1");

				HttpContextBase.Request.HttpMethod.Returns(verb);
				HttpContextBase.Request.ApplicationPath.Returns(applicationPath);
				HttpContextBase.Request.Url.Returns(uri);

				var inputStream = Substitute.For<Stream>();
				inputStream.CanRead.Returns(false);
				HttpContextBase.Request.InputStream.Returns(inputStream);

				RequestRouter.When(x => x.FindRequestHandler(verb, applicationPath, uri)).Do(_ => { throw new MalformedPayloadException(); });

				_piccoloContext = new PiccoloContext(HttpContextBase);

				Engine.ProcessRequest(_piccoloContext);
			}

			[Test]
			public void it_should_raise_request_faulted_event()
			{
				EventDispatcher.Received().RaiseRequestFaultedEvent(_piccoloContext, Arg.Any<MalformedPayloadException>());
			}

			[Test]
			public void it_should_return_status_code_422()
			{
				HttpContextBase.Response.StatusCode.Returns(422);
			}

			[Test]
			public void it_should_return_status_description_unprocessable_entity()
			{
				HttpContextBase.Response.StatusDescription.Returns("Unprocessable Entity");
			}

			[Test]
			public void it_should_not_return_response_payload()
			{
				HttpContextBase.Response.DidNotReceive().Write(Arg.Any<string>());
			}
		}

		[TestFixture]
		public class when_processing_request_that_sends_a_malformed_payload_and_http_debugging_is_enabled : given_piccolo_engine
		{
			private PiccoloContext _piccoloContext;

			[SetUp]
			public void SetUp()
			{
				const string verb = "POST";
				const string applicationPath = "/";
				var uri = new Uri("http://example.com/resources/1");

				HttpContextBase.Request.HttpMethod.Returns(verb);
				HttpContextBase.Request.ApplicationPath.Returns(applicationPath);
				HttpContextBase.Request.Url.Returns(uri);
				HttpContextBase.IsDebuggingEnabled.Returns(true);

				var inputStream = Substitute.For<Stream>();
				inputStream.CanRead.Returns(false);
				HttpContextBase.Request.InputStream.Returns(inputStream);

				RequestRouter.When(x => x.FindRequestHandler(verb, applicationPath, uri)).Do(_ => { throw new MalformedPayloadException(); });

				_piccoloContext = new PiccoloContext(HttpContextBase);

				Engine.ProcessRequest(_piccoloContext);
			}

			[Test]
			public void it_should_return_response_payload()
			{
				HttpContextBase.Response.Received().Write(Arg.Is<string>(x => x.Contains("MalformedPayloadException")));
			}
		}

		[TestFixture]
		public class when_processing_request_that_triggers_an_exception_and_http_debugging_is_disabled : given_piccolo_engine
		{
			private PiccoloContext _piccoloContext;

			[SetUp]
			public void SetUp()
			{
				const string verb = "POST";
				const string applicationPath = "/";
				var uri = new Uri("http://example.com/resources/1");

				HttpContextBase.Request.HttpMethod.Returns(verb);
				HttpContextBase.Request.ApplicationPath.Returns(applicationPath);
				HttpContextBase.Request.Url.Returns(uri);

				var inputStream = Substitute.For<Stream>();
				inputStream.CanRead.Returns(false);
				HttpContextBase.Request.InputStream.Returns(inputStream);

				RequestRouter.When(x => x.FindRequestHandler(verb, applicationPath, uri)).Do(_ => { throw new Exception(); });

				_piccoloContext = new PiccoloContext(HttpContextBase);

				Engine.ProcessRequest(_piccoloContext);
			}

			[Test]
			public void it_should_raise_request_faulted_event()
			{
				EventDispatcher.Received().RaiseRequestFaultedEvent(_piccoloContext, Arg.Any<Exception>());
			}

			[Test]
			public void it_should_return_status_code_500()
			{
				HttpContextBase.Response.StatusCode.Returns(500);
			}

			[Test]
			public void it_should_return_status_description_internal_server_error()
			{
				HttpContextBase.Response.StatusDescription.Returns("Internal Server Error");
			}

			[Test]
			public void it_should_not_return_response_payload()
			{
				HttpContextBase.Response.DidNotReceive().Write(Arg.Any<string>());
			}
		}

		[TestFixture]
		public class when_processing_request_that_triggers_an_exception_and_http_debugging_is_enabled : given_piccolo_engine
		{
			private PiccoloContext _piccoloContext;

			[SetUp]
			public void SetUp()
			{
				const string verb = "POST";
				const string applicationPath = "/";
				var uri = new Uri("http://example.com/resources/1");

				HttpContextBase.Request.HttpMethod.Returns(verb);
				HttpContextBase.Request.ApplicationPath.Returns(applicationPath);
				HttpContextBase.Request.Url.Returns(uri);
				HttpContextBase.IsDebuggingEnabled.Returns(true);

				var inputStream = Substitute.For<Stream>();
				inputStream.CanRead.Returns(false);
				HttpContextBase.Request.InputStream.Returns(inputStream);

				RequestRouter.When(x => x.FindRequestHandler(verb, applicationPath, uri)).Do(_ => { throw new Exception(); });

				_piccoloContext = new PiccoloContext(HttpContextBase);

				Engine.ProcessRequest(_piccoloContext);
			}

			[Test]
			public void it_should_return_response_payload()
			{
				HttpContextBase.Response.Received().Write(Arg.Is<string>(x => x.Contains("Exception")));
			}
		}

		[TestFixture]
		public class when_processing_request_that_is_interrupted_by_a_request_processing_event_handler : given_piccolo_engine
		{
			private PiccoloContext _piccoloContext;

			[SetUp]
			public void SetUp()
			{
				_piccoloContext = new PiccoloContext(HttpContextBase);

				EventDispatcher.RaiseRequestProcessingEvent(_piccoloContext).Returns(true);

				Engine.ProcessRequest(_piccoloContext);
			}

			[Test]
			public void it_should_not_continue_processing_request()
			{
				RequestRouter.DidNotReceive().FindRequestHandler(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Uri>());
			}

			[Test]
			public void it_should_raise_request_processed_event()
			{
				EventDispatcher.Received().RaiseRequestProcessedEvent(_piccoloContext, Arg.Any<string>());
			}
		}

		[TestFixture]
		public class when_processing_request_with_an_invalid_validator : given_piccolo_engine
		{
			private PiccoloContext _piccoloContext;

			[SetUp]
			public void SetUp()
			{
				const string verb = "POST";
				const string applicationPath = "/";
				var uri = new Uri("http://example.com/resources/1");

				HttpContextBase.Request.HttpMethod.Returns(verb);
				HttpContextBase.Request.ApplicationPath.Returns(applicationPath);
				HttpContextBase.Request.Url.Returns(uri);
				HttpContextBase.IsDebuggingEnabled.Returns(true);

				var inputStream = Substitute.For<Stream>();
				inputStream.CanRead.Returns(false);
				HttpContextBase.Request.InputStream.Returns(inputStream);

				RequestRouter.When(x => x.FindRequestHandler(verb, applicationPath, uri)).Do(_ => { throw new MalformedPayloadException(); });

				_piccoloContext = new PiccoloContext(HttpContextBase);

				Engine.ProcessRequest(_piccoloContext);
			}

			[Test]
			public void it_should_return_response_payload()
			{
				HttpContextBase.Response.Received().Write(Arg.Is<string>(x => x.Contains("MalformedPayloadException")));
			}
		}

		[TestFixture]
		public class when_processing_request_that_fails_payload_validation : given_piccolo_engine
		{
			private PiccoloContext _piccoloContext;

			[SetUp]
			public void SetUp()
			{
				const string verb = "POST";
				const string applicationPath = "/";
				var uri = new Uri("http://example.com/resources");
				var routeParameters = new Dictionary<string, string>();

				HttpContextBase.Request.HttpMethod.Returns(verb);
				HttpContextBase.Request.ApplicationPath.Returns(applicationPath);
				HttpContextBase.Request.Url.Returns(uri);
				HttpContextBase.Request.InputStream.Returns(new MemoryStream(Encoding.UTF8.GetBytes("\"request_payload\"")));

				RequestRouter.FindRequestHandler(verb, applicationPath, uri).Returns(new RouteHandlerLookupResult(typeof(CreateResource), routeParameters));

				var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.Created);
				httpResponseMessage.Headers.Location = new Uri("http://example.com/resources/1");

				RequestHandlerInvoker.Execute(
					Arg.Any<CreateResource>(),
					verb,
					routeParameters,
					Arg.Is<IDictionary<string, string>>(x => x.Count == 0),
					Arg.Any<IDictionary<string, object>>(),
					Arg.Is<string>(x => x == "request_payload")).Returns(httpResponseMessage);

				_piccoloContext = new PiccoloContext(HttpContextBase);

				Engine.ProcessRequest(_piccoloContext);
			}

			[Test]
			public void it_should_return_status_code_400()
			{
				HttpContextBase.Response.StatusCode.Returns(400);
			}

			[Test]
			public void it_should_return_status_description_bad_request()
			{
				HttpContextBase.Response.StatusDescription.Returns("Bad Request");
			}

			[Test]
			public void it_should_return_response_payload()
			{
				HttpContextBase.Response.Received().Write(Arg.Is<string>(x => x.Contains("FAIL")));
			}

			[ExcludeFromCodeCoverage]
			[ValidateWith(typeof(Validator))]
			public class CreateResource : IPost<string, string>
			{
				public HttpResponseMessage<string> Post(string parameters)
				{
					return null;
				}
			}

			public class Validator : IPayloadValidator<string>
			{
				public ValidationResult Validate(string payload)
				{
					return new ValidationResult("FAIL");
				}
			}
		}
	}

	public abstract class given_piccolo_engine
	{
		protected IEventDispatcher EventDispatcher;
		protected IRequestRouter RequestRouter;
		protected IRequestHandlerInvoker RequestHandlerInvoker;
		protected PiccoloEngine Engine;
		protected HttpContextBase HttpContextBase;

		protected given_piccolo_engine()
		{
			var piccoloConfiguration = Bootstrapper.ApplyConfiguration(Assembly.GetCallingAssembly(), false);
			EventDispatcher = Substitute.For<IEventDispatcher>();
			RequestRouter = Substitute.For<IRequestRouter>();
			RequestHandlerInvoker = Substitute.For<IRequestHandlerInvoker>();
			HttpContextBase = Substitute.For<HttpContextBase>();

			Engine = new PiccoloEngine(piccoloConfiguration, EventDispatcher, RequestRouter, RequestHandlerInvoker);
		}
	}
}