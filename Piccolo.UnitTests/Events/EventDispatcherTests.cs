using System.Diagnostics.CodeAnalysis;
using System.Web;
using NSubstitute;
using NUnit.Framework;
using Piccolo.Configuration;
using Piccolo.Events;
using Piccolo.UnitTests.Configuration;

namespace Piccolo.UnitTests.Events
{
	public class EventDispatcherTests
	{
		[TestFixture]
		public class when_request_processing_event_is_raised_with_two_handlers_and_first_stop_execution
		{
			private HttpResponseBase _httpResponse;

			[TestFixtureSetUp]
			public void SetUp()
			{
				var eventHandlers = new EventHandlers();
				eventHandlers.RequestProcessing.Add(typeof(TestRequestProcessingEventHandlerWithInterrupt));
				eventHandlers.RequestProcessing.Add(typeof(BootstrapperTests.TestRequestProcessingEventHandler));

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
				_httpResponse.Received().Write("RequestProcessingEvent handled with interrupt");
			}

			[Test]
			public void it_should_not_execute_second_handler()
			{
				_httpResponse.DidNotReceive().Write("RequestProcessingEvent handled");
			}
		}

		#region Test Classes

		[ExcludeFromCodeCoverage]
		public class TestRequestProcessingEventHandlerWithInterrupt : IHandle<RequestProcessingEvent>
		{
			public void Handle(RequestProcessingEvent args)
			{
				args.Context.Http.Response.Write("RequestProcessingEvent handled with interrupt");
				args.StopProcessing = true;
			}
		}

		#endregion
	}
}