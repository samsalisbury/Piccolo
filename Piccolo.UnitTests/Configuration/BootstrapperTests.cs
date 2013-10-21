using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Piccolo.Configuration;
using Piccolo.Events;
using Piccolo.UnitTests.Events;
using Shouldly;

namespace Piccolo.UnitTests.Configuration
{
	public class BootstrapperTests
	{
		[TestFixture]
		public class when_instantiated_with_default_configuration
		{
			private PiccoloConfiguration _handlerConfiguration;

			[SetUp]
			public void SetUp()
			{
				_handlerConfiguration = Bootstrapper.ApplyConfiguration(Assembly.GetExecutingAssembly(), false);
			}

			[Test]
			public void it_should_configure_default_object_factory()
			{
				_handlerConfiguration.ObjectFactory.ShouldBeTypeOf<DefaultObjectFactory>();
			}

			[Test]
			public void it_should_autodetect_request_handlers()
			{
				_handlerConfiguration.RequestHandlers.Any(x => x == typeof(TestRequestHandler)).ShouldBe(true);
			}

			[Test]
			public void it_should_autodetect_request_processing_event_handlers()
			{
				_handlerConfiguration.EventHandlers.RequestProcessing[0].ShouldBe(typeof(EventDispatcherTests.TestRequestProcessingEventHandler));
			}

			[Test]
			public void it_should_autodetect_request_processed_event_handlers()
			{
				_handlerConfiguration.EventHandlers.RequestProcessed[0].ShouldBe(typeof(EventDispatcherTests.TestRequestProcessedEventHandlerWithStopEventProcessing));
			}

			[Test]
			public void it_should_autodetect_request_faulted_event_handlers()
			{
				_handlerConfiguration.EventHandlers.RequestFaulted[0].ShouldBe(typeof(TestRequestFaultedEventHandler));
			}

			[Test]
			public void it_should_configure_json_serialiser()
			{
				_handlerConfiguration.JsonSerialiser("Test").ShouldBe("\"Test\"");
			}

			[Test]
			public void it_should_configure_json_deserialiser()
			{
				_handlerConfiguration.JsonDeserialiser(typeof(string), "\"Test\"").ShouldBe("Test");
			}
		}

		[TestFixture]
		public class when_executed_with_custom_configuration_enabled
		{
			private PiccoloConfiguration _handlerConfiguration;

			[SetUp]
			public void SetUp()
			{
				_handlerConfiguration = Bootstrapper.ApplyConfiguration(Assembly.GetExecutingAssembly(), true);
			}

			[Test]
			public void it_should_configure_custom_request_handler_factory()
			{
				_handlerConfiguration.ObjectFactory.ShouldBeTypeOf<CustomObjectFactory>();
			}
		}

		#region Test Classes

		public class StartupTask : IStartupTask
		{
			public void Run(PiccoloConfiguration configuration)
			{
				configuration.ObjectFactory = new CustomObjectFactory();
			}
		}

		[ExcludeFromCodeCoverage]
		public class CustomObjectFactory : IObjectFactory
		{
			public T CreateInstance<T>(Type requestHandlerType)
			{
				return default(T);
			}
		}

		[ExcludeFromCodeCoverage]
		[Priority(1)]
		public class TestRequestFaultedEventHandler : IHandle<RequestFaultedEvent>
		{
			public void Handle(RequestFaultedEvent args)
			{
				args.Context.Http.Response.Write("RequestFaultedEvent handled+" + args.Exception.GetType().Name);
			}
		}

		#endregion
	}
}