using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web;
using NUnit.Framework;
using Piccolo.Configuration;
using Piccolo.Events;
using Shouldly;

namespace Piccolo.Tests.Configuration
{
	public class BootstrapperTests
	{
		[TestFixture]
		public class when_executed_with_default_configuration
		{
			private PiccoloConfiguration _handlerConfiguration;

			[SetUp]
			public void SetUp()
			{
				_handlerConfiguration = Bootstrapper.ApplyConfiguration(Assembly.GetExecutingAssembly(), false);
			}

			[Test]
			public void it_should_autodetect_request_handlers()
			{
				_handlerConfiguration.RequestHandlers.Any(x => x == typeof(TestRequestHandler)).ShouldBe(true);
			}

			[Test]
			public void it_should_autodetect_request_processing_event_handlers()
			{
				_handlerConfiguration.EventHandlers.RequestProcessing.First().GetInterfaces().First().ShouldBe(typeof(IHandle<RequestProcessingEvent>));
			}

			[Test]
			public void it_should_autodetect_request_processed_event_handlers()
			{
				_handlerConfiguration.EventHandlers.RequestProcessed.First().GetInterfaces().First().ShouldBe(typeof(IHandle<RequestProcessedEvent>));
			}

			[Test]
			public void it_should_autodetect_request_faulted_event_handlers()
			{
				_handlerConfiguration.EventHandlers.RequestFaulted.First().GetInterfaces().First().ShouldBe(typeof(IHandle<RequestFaultedEvent>));
			}

			[Test]
			public void it_should_configure_default_object_factory()
			{
				_handlerConfiguration.ObjectFactory.ShouldBeTypeOf<DefaultObjectFactory>();
			}

			[Test]
			public void it_should_configure_string_parameter_binder()
			{
				_handlerConfiguration.ParameterBinders[typeof(string)]("test").ShouldBe("test");
			}

			[Test]
			public void it_should_configure_boolean_parameter_binder()
			{
				_handlerConfiguration.ParameterBinders[typeof(bool)]("true").ShouldBe(true);
			}

			[Test]
			public void it_should_configure_nullable_boolean_parameter_binder()
			{
				_handlerConfiguration.ParameterBinders[typeof(bool?)]("true").ShouldBe(true);
			}

			[Test]
			public void it_should_configure_integer_parameter_binder()
			{
				_handlerConfiguration.ParameterBinders[typeof(int)]("1").ShouldBe(1);
			}

			[Test]
			public void it_should_configure_nullable_integer_parameter_binder()
			{
				_handlerConfiguration.ParameterBinders[typeof(int?)]("1").ShouldBe(1);
			}

			[Test]
			public void it_should_configure_datetime_parameter_binder()
			{
				_handlerConfiguration.ParameterBinders[typeof(DateTime)]("01/01/1970 12:00:00").ShouldBe(new DateTime(1970, 01, 01, 12, 00, 00));
			}

			[Test]
			public void it_should_configure_nullable_datetime_parameter_binder()
			{
				_handlerConfiguration.ParameterBinders[typeof(DateTime?)]("01/01/1970 12:00:00").ShouldBe(new DateTime(1970, 01, 01, 12, 00, 00));
			}

			[Test]
			public void it_should_configure_json_serialiser()
			{
				_handlerConfiguration.JsonSerialiser(new {}).ShouldBe("{}");
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

		[TestFixture]
		public class when_executed_with_assembly_that_does_not_contain_global_asax
		{
			[Test]
			public void it_should_throw_exception()
			{
				Should.Throw<InvalidOperationException>(() => Bootstrapper.ApplyConfiguration(typeof(HttpApplication).BaseType.Assembly, false));
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

		[Route("")]
		[ExcludeFromCodeCoverage]
		public class TestRequestHandler : IGet<string>
		{
			public HttpResponseMessage<string> Get()
			{
				return new HttpResponseMessage<string>(new HttpResponseMessage());
			}
		}

		[ExcludeFromCodeCoverage]
		public class TestRequestProcessingEventHandler : IHandle<RequestProcessingEvent>
		{
			public void Handle(RequestProcessingEvent args)
			{
			}
		}

		[ExcludeFromCodeCoverage]
		public class TestRequestFaultedEventHandler : IHandle<RequestFaultedEvent>
		{
			public void Handle(RequestFaultedEvent args)
			{
			}
		}

		[ExcludeFromCodeCoverage]
		public class TestRequestProcessedEventHandler : IHandle<RequestProcessedEvent>
		{
			public void Handle(RequestProcessedEvent args)
			{
			}
		}

		#endregion
	}
}