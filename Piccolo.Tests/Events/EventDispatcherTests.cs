using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Web;
using NSubstitute;
using NUnit.Framework;
using Piccolo.Configuration;
using Piccolo.Events;
using Shouldly;

namespace Piccolo.Tests.Events
{
	public class EventDispatcherTests
	{
		[TestFixture]
		public class when_request_processing_event_is_raised : given_event_dispatcher
		{
			[TestFixtureSetUp]
			public void SetUp()
			{
				var eventHandlers = new EventHandlers(new List<Type> {typeof(EventHandler1), typeof(EventHandler2)}, new List<Type>(), new List<Type>());
				var dispatcher = new EventDispatcher(eventHandlers, new DefaultObjectFactory());

				dispatcher.RaiseRequestProcessingEvent(PiccoloContext);
			}

			[Test]
			public void it_should_execute_first_event_handler()
			{
				HttpResponse.Received().Write("EventHandler1");
			}

			[Test]
			public void it_should_execute_second_event_handler()
			{
				HttpResponse.Received().Write("EventHandler2");
			}

			[ExcludeFromCodeCoverage]
			public class EventHandler1 : IHandle<RequestProcessingEvent>
			{
				public void Handle(RequestProcessingEvent args)
				{
					args.Context.Http.Response.Write("EventHandler1");
				}
			}

			[ExcludeFromCodeCoverage]
			public class EventHandler2 : IHandle<RequestProcessingEvent>
			{
				public void Handle(RequestProcessingEvent args)
				{
					args.Context.Http.Response.Write("EventHandler2");
				}
			}
		}

		[TestFixture]
		public class when_request_faulted_event_is_raised : given_event_dispatcher
		{
			[TestFixtureSetUp]
			public void SetUp()
			{
				var eventHandlers = new EventHandlers(new List<Type>(), new List<Type> {typeof(EventHandler1), typeof(EventHandler2)}, new List<Type>());
				var dispatcher = new EventDispatcher(eventHandlers, new DefaultObjectFactory());

				dispatcher.RaiseRequestFaultedEvent(PiccoloContext, new Exception());
			}

			[Test]
			public void it_should_execute_first_event_handler()
			{
				HttpResponse.Received().Write("EventHandler1: Exception");
			}

			[Test]
			public void it_should_execute_second_event_handler()
			{
				HttpResponse.Received().Write("EventHandler2: Exception");
			}

			[ExcludeFromCodeCoverage]
			public class EventHandler1 : IHandle<RequestFaultedEvent>
			{
				public void Handle(RequestFaultedEvent args)
				{
					args.Context.Http.Response.Write("EventHandler1: " + args.Exception.GetType().Name);
				}
			}

			[ExcludeFromCodeCoverage]
			public class EventHandler2 : IHandle<RequestFaultedEvent>
			{
				public void Handle(RequestFaultedEvent args)
				{
					args.Context.Http.Response.Write("EventHandler2: " + args.Exception.GetType().Name);
				}
			}
		}

		[TestFixture]
		public class when_request_processed_event_is_raised : given_event_dispatcher
		{
			[TestFixtureSetUp]
			public void SetUp()
			{
				var eventHandlers = new EventHandlers(new List<Type>(), new List<Type>(), new List<Type> {typeof(EventHandler1), typeof(EventHandler2)});
				var dispatcher = new EventDispatcher(eventHandlers, new DefaultObjectFactory());

				dispatcher.RaiseRequestProcessedEvent(PiccoloContext, "payload");
			}

			[Test]
			public void it_should_execute_first_event_handler()
			{
				HttpResponse.Received().Write("EventHandler1: payload");
			}

			[Test]
			public void it_should_execute_second_event_handler()
			{
				HttpResponse.Received().Write("EventHandler2: payload");
			}

			[ExcludeFromCodeCoverage]
			public class EventHandler1 : IHandle<RequestProcessedEvent>
			{
				public void Handle(RequestProcessedEvent args)
				{
					args.Context.Http.Response.Write("EventHandler1: " + args.Payload);
				}
			}

			[ExcludeFromCodeCoverage]
			public class EventHandler2 : IHandle<RequestProcessedEvent>
			{
				public void Handle(RequestProcessedEvent args)
				{
					args.Context.Http.Response.Write("EventHandler2: " + args.Payload);
				}
			}
		}

		[TestFixture]
		public class when_request_processing_event_is_raised_and_first_handler_stops_event_processing : given_event_dispatcher
		{
			private bool _stopRequestProcessing;

			[TestFixtureSetUp]
			public void SetUp()
			{
				var eventHandlers = new EventHandlers(new List<Type> {typeof(EventHandler1), typeof(EventHandler2)}, new List<Type>(), new List<Type>());
				var dispatcher = new EventDispatcher(eventHandlers, new DefaultObjectFactory());

				_stopRequestProcessing = dispatcher.RaiseRequestProcessingEvent(PiccoloContext);
			}

			[Test]
			public void it_should_execute_first_handler()
			{
				HttpResponse.Received().Write("EventHandler1");
			}

			[Test]
			public void it_should_not_execute_second_handler()
			{
				HttpResponse.DidNotReceive().Write("EventHandler2");
			}

			[Test]
			public void it_should_not_stop_request_processing()
			{
				_stopRequestProcessing.ShouldBe(false);
			}

			[ExcludeFromCodeCoverage]
			public class EventHandler1 : IHandle<RequestProcessingEvent>
			{
				public void Handle(RequestProcessingEvent args)
				{
					args.Context.Http.Response.Write("EventHandler1");
					args.StopEventProcessing = true;
				}
			}

			[ExcludeFromCodeCoverage]
			public class EventHandler2 : IHandle<RequestProcessingEvent>
			{
				public void Handle(RequestProcessingEvent args)
				{
					args.Context.Http.Response.Write("EventHandler2");
				}
			}
		}

		[TestFixture]
		public class when_request_faulted_event_is_raised_and_first_handler_stops_event_processing : given_event_dispatcher
		{
			[TestFixtureSetUp]
			public void SetUp()
			{
				var eventHandlers = new EventHandlers(new List<Type>(), new List<Type> {typeof(EventHandler1), typeof(EventHandler2)}, new List<Type>());
				var dispatcher = new EventDispatcher(eventHandlers, new DefaultObjectFactory());

				dispatcher.RaiseRequestFaultedEvent(PiccoloContext, new Exception());
			}

			[Test]
			public void it_should_execute_first_handler()
			{
				HttpResponse.Received().Write("EventHandler1");
			}

			[Test]
			public void it_should_not_execute_second_handler()
			{
				HttpResponse.DidNotReceive().Write("EventHandler2");
			}

			[ExcludeFromCodeCoverage]
			public class EventHandler1 : IHandle<RequestFaultedEvent>
			{
				public void Handle(RequestFaultedEvent args)
				{
					args.Context.Http.Response.Write("EventHandler1");
					args.StopEventProcessing = true;
				}
			}

			[ExcludeFromCodeCoverage]
			public class EventHandler2 : IHandle<RequestFaultedEvent>
			{
				public void Handle(RequestFaultedEvent args)
				{
					args.Context.Http.Response.Write("EventHandler2");
				}
			}
		}

		[TestFixture]
		public class when_request_processed_event_is_raised_and_first_handler_stops_event_processing : given_event_dispatcher
		{
			[TestFixtureSetUp]
			public void SetUp()
			{
				var eventHandlers = new EventHandlers(new List<Type>(), new List<Type>(), new List<Type> {typeof(EventHandler1), typeof(EventHandler2)});
				var dispatcher = new EventDispatcher(eventHandlers, new DefaultObjectFactory());

				dispatcher.RaiseRequestProcessedEvent(PiccoloContext, null);
			}

			[Test]
			public void it_should_execute_first_handler()
			{
				HttpResponse.Received().Write("EventHandler1");
			}

			[Test]
			public void it_should_not_execute_second_handler()
			{
				HttpResponse.DidNotReceive().Write("EventHandler2");
			}

			[ExcludeFromCodeCoverage]
			public class EventHandler1 : IHandle<RequestProcessedEvent>
			{
				public void Handle(RequestProcessedEvent args)
				{
					args.Context.Http.Response.Write("EventHandler1");
					args.StopEventProcessing = true;
				}
			}

			[ExcludeFromCodeCoverage]
			public class EventHandler2 : IHandle<RequestProcessedEvent>
			{
				public void Handle(RequestProcessedEvent args)
				{
					args.Context.Http.Response.Write("EventHandler2");
				}
			}
		}

		[TestFixture]
		public class when_request_processing_event_is_raised_and_first_handler_stops_request_processing : given_event_dispatcher
		{
			private bool _stopRequestProcessing;

			[TestFixtureSetUp]
			public void SetUp()
			{
				var eventHandlers = new EventHandlers(new List<Type> {typeof(EventHandler1), typeof(EventHandler2)}, new List<Type>(), new List<Type>());
				var dispatcher = new EventDispatcher(eventHandlers, new DefaultObjectFactory());

				_stopRequestProcessing = dispatcher.RaiseRequestProcessingEvent(PiccoloContext);
			}

			[Test]
			public void it_should_execute_first_handler()
			{
				HttpResponse.Received().Write("EventHandler1");
			}

			[Test]
			public void it_should_execute_second_handler()
			{
				HttpResponse.Received().Write("EventHandler2");
			}

			[Test]
			public void it_should_stop_request_processing()
			{
				_stopRequestProcessing.ShouldBe(true);
			}

			[ExcludeFromCodeCoverage]
			public class EventHandler1 : IHandle<RequestProcessingEvent>
			{
				public void Handle(RequestProcessingEvent args)
				{
					args.Context.Http.Response.Write("EventHandler1");
					args.StopRequestProcessing = true;
				}
			}

			[ExcludeFromCodeCoverage]
			public class EventHandler2 : IHandle<RequestProcessingEvent>
			{
				public void Handle(RequestProcessingEvent args)
				{
					args.Context.Http.Response.Write("EventHandler2");
				}
			}
		}

		public abstract class given_event_dispatcher
		{
			protected HttpResponseBase HttpResponse = Substitute.For<HttpResponseBase>();
			protected PiccoloContext PiccoloContext;

			protected given_event_dispatcher()
			{
				var httpContext = Substitute.For<HttpContextBase>();
				httpContext.Response.Returns(HttpResponse);

				PiccoloContext = new PiccoloContext(httpContext);
			}
		}
	}
}