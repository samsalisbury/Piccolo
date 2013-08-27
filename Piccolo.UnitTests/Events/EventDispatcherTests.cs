using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Web;
using Moq;
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
			private Mock<HttpResponseBase> _httpResponse;

			[TestFixtureSetUp]
			public void SetUp()
			{
				var eventDispatcher = new EventDispatcher(new EventHandlers
				{
					RequestProcessing = new[]
					{
						typeof(TestRequestProcessingEventHandlerWithInterrupt),
						typeof(BootstrapperTests.TestRequestProcessingEventHandler)
					}
				});

				var httpContext = new Mock<HttpContextBase>();
				_httpResponse = new Mock<HttpResponseBase>();
				httpContext.SetupGet(x => x.Response).Returns(_httpResponse.Object);
				var piccoloContext = new PiccoloContext(httpContext.Object);

				eventDispatcher.RaiseRequestProcessingEvent(piccoloContext);
			}

			[Test]
			public void it_should_execute_first_handler()
			{
				_httpResponse.Verify(x => x.Write("RequestProcessingEvent handled with interrupt"), Times.Once());
			}

			[Test]
			public void it_should_not_execute_second_handler()
			{
				_httpResponse.Verify(x => x.Write("RequestProcessingEvent handled"), Times.Never());
			}
		}

		[ExcludeFromCodeCoverage]
		public class TestRequestProcessingEventHandlerWithInterrupt : IHandle<RequestProcessingEvent>
		{
			public void Handle(RequestProcessingEvent args)
			{
				args.Context.Http.Response.Write("RequestProcessingEvent handled with interrupt");
				args.StopProcessing = true;
			}
		}
	}
}