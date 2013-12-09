using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web;
using NSubstitute;
using NUnit.Framework;
using Piccolo.Configuration;
using Piccolo.Events;
using Piccolo.Request;
using Piccolo.Routing;
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
				HttpContextBase.Request.InputStream.Returns(new MemoryStream());

				RequestRouter.FindRequestHandler(verb, applicationPath, uri).Returns(new RouteHandlerLookupResult(typeof(GetResource), routeParameters));

				RequestHandlerInvoker.Execute(
					Arg.Any<GetResource>(),
					verb,
					routeParameters,
					Arg.Is<IDictionary<string, string>>(x => x["query"] == "2"),
					Arg.Is<IDictionary<string, object>>(x => (string)x["context"] == "3"),
					Arg.Is<string>(x => x == string.Empty),
					Arg.Is<object>(x => x == null)).Returns(new HttpResponseMessage {Content = new ObjectContent("test")});

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
				HttpContextBase.Request.InputStream.Returns(new MemoryStream());

				RequestRouter.FindRequestHandler(verb, applicationPath, uri).Returns(new RouteHandlerLookupResult(typeof(DeleteResource), routeParameters));

				RequestHandlerInvoker.Execute(
					Arg.Any<DeleteResource>(),
					verb,
					routeParameters,
					Arg.Is<IDictionary<string, string>>(x => x.Count == 0),
					Arg.Any<IDictionary<string, object>>(),
					Arg.Is<string>(x => x == string.Empty),
					Arg.Is<object>(x => x == null)).Returns(new HttpResponseMessage(HttpStatusCode.NoContent));

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
		public class when_processing_request_that_set_location_header : given_piccolo_engine
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
				HttpContextBase.Request.InputStream.Returns(new MemoryStream());

				RequestRouter.FindRequestHandler(verb, applicationPath, uri).Returns(new RouteHandlerLookupResult(typeof(CreateResource), routeParameters));

				var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.Created);
				httpResponseMessage.Headers.Location = new Uri("http://example.com/resources/1");

				RequestHandlerInvoker.Execute(
					Arg.Any<CreateResource>(),
					verb,
					routeParameters,
					Arg.Is<IDictionary<string, string>>(x => x.Count == 0),
					Arg.Any<IDictionary<string, object>>(),
					Arg.Is<string>(x => x == string.Empty),
					Arg.Is<object>(x => x == null)).Returns(httpResponseMessage);

				_piccoloContext = new PiccoloContext(HttpContextBase);

				Engine.ProcessRequest(_piccoloContext);
			}

			[Test]
			public void it_should_set_location_response_header()
			{
				HttpContextBase.Response.Received().AddHeader("Location", "http://example.com/resources/1");
			}

			[ExcludeFromCodeCoverage]
			public class CreateResource : IGet<string>
			{
				public HttpResponseMessage<string> Get()
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
				const string verb = "GET";
				const string applicationPath = "/";
				var uri = new Uri("http://example.com/resources/1");
				var routeParameters = new Dictionary<string, string> { { "route", "1" } };

				HttpContextBase.Request.HttpMethod.Returns(verb);
				HttpContextBase.Request.ApplicationPath.Returns(applicationPath);
				HttpContextBase.Request.Url.Returns(uri);
				HttpContextBase.Request.InputStream.Returns(new MemoryStream());

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

			[ExcludeFromCodeCoverage]
			public class GetResource : IGet<string>
			{
				public HttpResponseMessage<string> Get()
				{
					return null;
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