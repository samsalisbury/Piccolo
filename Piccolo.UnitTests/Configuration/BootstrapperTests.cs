using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Piccolo.Configuration;
using Shouldly;

namespace Piccolo.UnitTests.Configuration
{
	public class BootstrapperTests
	{
		[TestFixture]
		public class when_instantiated_with_default_configuration : given_bootstrapper
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
		public class when_executed_with_custom_configuration_enabled : given_bootstrapper
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

		public abstract class given_bootstrapper
		{
			protected Bootstrapper Bootstrapper;

			protected given_bootstrapper()
			{
				Bootstrapper = new Bootstrapper();
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

		#endregion
	}
}