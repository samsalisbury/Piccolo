using System.Diagnostics.CodeAnalysis;
using System.Web;
using NSubstitute;
using NUnit.Framework;
using Piccolo.Configuration;
using Piccolo.Events;
using Shouldly;

namespace Piccolo.UnitTests.Events
{
	public class EventDispatcherTests
	{
		[TestFixture]
		public class when_request_processing_event_is_raised_with_two_handlers_and_first_stops_event_processing
		{
			private HttpResponseBase _httpResponse;

			[TestFixtureSetUp]
			public void SetUp()
			{
				var eventHandlers = new EventHandlers();
				eventHandlers.RequestProcessing.Add(typeof(TestRequestProcessingEventHandlerWithStopEventProcessing));
				eventHandlers.RequestProcessing.Add(typeof(TestRequestProcessingEventHandler));

				var httpContext = Substitute.For<HttpContextBase>();
				_httpResponse = Substitute.For<HttpResponseBase>();
				httpContext.Response.Returns(_httpResponse);
				var piccoloContext = new PiccoloContext(httpContext);

				var eventDispatcher = new EventDispatcher(eventHandlers, new DefaultObjectFactory());
				eventDispatcher.RaiseRequestProcessingEvent(piccoloContext);
			}

			[Test]
			public void it_should_execute_first_handler()
			{
				_httpResponse.Received().Write("RequestProcessingEvent handled with StopEventProcessing");
			}

			[Test]
			public void it_should_not_execute_second_handler()
			{
				_httpResponse.DidNotReceive().Write("RequestProcessingEvent handled");
			}
		}

		[TestFixture]
		public class when_request_processed_event_is_raised_with_two_handlers_and_first_stops_event_processing
		{
			private HttpResponseBase _httpResponse;

			[TestFixtureSetUp]
			public void SetUp()
			{
				var eventHandlers = new EventHandlers();
				eventHandlers.RequestProcessed.Add(typeof(TestRequestProcessedEventHandlerWithStopEventProcessing));
				eventHandlers.RequestProcessed.Add(typeof(TestRequestProcessingEventHandler));

				var httpContext = Substitute.For<HttpContextBase>();
				_httpResponse = Substitute.For<HttpResponseBase>();
				httpContext.Response.Returns(_httpResponse);
				var piccoloContext = new PiccoloContext(httpContext);

				var eventDispatcher = new EventDispatcher(eventHandlers, new DefaultObjectFactory());
				eventDispatcher.RaiseRequestProcessedEvent(piccoloContext);
			}

			[Test]
			public void it_should_execute_first_handler()
			{
				_httpResponse.Received().Write("RequestProcessedEvent handled with StopEventProcessing");
			}

			[Test]
			public void it_should_not_execute_second_handler()
			{
				_httpResponse.DidNotReceive().Write("RequestProcessingEvent handled");
			}
		}

		[TestFixture]
		public class when_request_processing_event_is_raised_with_two_handlers_and_first_stops_request_processing
		{
			private HttpResponseBase _httpResponse;
			private bool _stopRequestProcessing;

			[TestFixtureSetUp]
			public void SetUp()
			{
				var eventHandlers = new EventHandlers();
				eventHandlers.RequestProcessing.Add(typeof(TestRequestProcessingEventHandlerWithStopRequestProcessing));
				eventHandlers.RequestProcessing.Add(typeof(TestRequestProcessingEventHandler));

				var httpContext = Substitute.For<HttpContextBase>();
				_httpResponse = Substitute.For<HttpResponseBase>();
				httpContext.Response.Returns(_httpResponse);
				var piccoloContext = new PiccoloContext(httpContext);

				var eventDispatcher = new EventDispatcher(eventHandlers, new DefaultObjectFactory());
				_stopRequestProcessing = eventDispatcher.RaiseRequestProcessingEvent(piccoloContext);
			}

			[Test]
			public void it_should_execute_first_handler()
			{
				_httpResponse.Received().Write("RequestProcessingEvent handled with StopRequestProcessing");
			}

			[Test]
			public void it_should_execute_second_handler()
			{
				_httpResponse.Received().Write("RequestProcessingEvent handled");
			}

			[Test]
			public void it_should_stop_request_processing()
			{
				_stopRequestProcessing.ShouldBe(true);
			}
		}

		#region Test Classes

		[ExcludeFromCodeCoverage]
		public class TestRequestProcessingEventHandlerWithStopEventProcessing : IHandle<RequestProcessingEvent>
		{
			public void Handle(RequestProcessingEvent args)
			{
				args.Context.Http.Response.Write("RequestProcessingEvent handled with StopEventProcessing");
				args.StopEventProcessing = true;
			}
		}

		[ExcludeFromCodeCoverage]
		[Priority(1)]
		public class TestRequestProcessedEventHandlerWithStopEventProcessing : IHandle<RequestProcessedEvent>
		{
			public void Handle(RequestProcessedEvent args)
			{
				args.Context.Http.Response.Write("RequestProcessedEvent handled with StopEventProcessing");
				args.StopEventProcessing = true;
			}
		}

		[ExcludeFromCodeCoverage]
		[Priority(1)]
		public class TestRequestProcessingEventHandler : IHandle<RequestProcessingEvent>
		{
			public void Handle(RequestProcessingEvent args)
			{
				args.Context.Http.Response.Write("RequestProcessingEvent handled");
			}
		}

		[ExcludeFromCodeCoverage]
		public class TestRequestProcessedEventHandler : IHandle<RequestProcessedEvent>
		{
			public void Handle(RequestProcessedEvent args)
			{
				args.Context.Http.Response.Write("RequestProcessedEvent handled");
			}
		}

		[ExcludeFromCodeCoverage]
		public class TestRequestProcessingEventHandlerWithStopRequestProcessing : IHandle<RequestProcessingEvent>
		{
			public void Handle(RequestProcessingEvent args)
			{
				args.Context.Http.Response.Write("RequestProcessingEvent handled with StopRequestProcessing");
				args.StopRequestProcessing = true;

				args.Context.Http.Response.StatusCode = 426;
			}
		}

		#endregion
	}
}