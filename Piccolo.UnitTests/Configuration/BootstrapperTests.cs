using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Piccolo.Configuration;
using Piccolo.Events;
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
			public void it_should_configure_default_request_handler_factory()
			{
				_handlerConfiguration.RequestHandlerFactory.ShouldBeTypeOf<DefaultRequestHandlerFactory>();
			}

			[Test]
			public void it_should_autodetect_request_handlers()
			{
				_handlerConfiguration.RequestHandlers.Any(x => x == typeof(TestRequestHandler)).ShouldBe(true);
			}

			[Test]
			public void it_should_autodetect_request_processing_event_handlers()
			{
				_handlerConfiguration.EventHandlers.RequestProcessing.Any(x => x == typeof(TestRequestProcessingEventHandler)).ShouldBe(true);
			}

			[Test]
			public void it_should_autodetect_request_processed_event_handlers()
			{
				_handlerConfiguration.EventHandlers.RequestProcessed.Any(x => x == typeof(TestRequestProcessedEventHandler)).ShouldBe(true);
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
				_handlerConfiguration.RequestHandlerFactory.ShouldBeTypeOf<CustomRequestHandlerFactory>();
			}
		}

		#region Test Classes

		public class StartupTask : IStartupTask
		{
			public void Run(PiccoloConfiguration configuration)
			{
				configuration.RequestHandlerFactory = new CustomRequestHandlerFactory();
			}
		}

		[ExcludeFromCodeCoverage]
		public class CustomRequestHandlerFactory : IRequestHandlerFactory
		{
			public IRequestHandler CreateInstance(Type requestHandlerType)
			{
				return null;
			}
		}

		[ExcludeFromCodeCoverage]
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

		#endregion
	}
}