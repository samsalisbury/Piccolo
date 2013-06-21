using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Piccolo.Configuration;
using Piccolo.IoC;
using Shouldly;

namespace Piccolo.UnitTests.Configuration
{
	public class BootstrapperTests
	{
		[TestFixture]
		public class when_instantiated_with_default_configuration : given_bootstrapper
		{
			private HttpHandlerConfiguration _handlerConfiguration;

			[SetUp]
			public void SetUp()
			{
				_handlerConfiguration = Bootstrapper.ApplyConfiguration(false);
			}

			[Test]
			public void it_should_configure_default_request_handler_factory()
			{
				_handlerConfiguration.RequestHandlerFactory.ShouldBeTypeOf<DefaultRequestHandlerFactory>();
			}
		}

		[TestFixture]
		public class when_executed_with_custom_configuration_enabled : given_bootstrapper
		{
			private HttpHandlerConfiguration _handlerConfiguration;

			[SetUp]
			public void SetUp()
			{
				_handlerConfiguration = Bootstrapper.ApplyConfiguration(true);
			}

			[Test]
			public void it_should_configure_custom_request_handler_factory()
			{
				_handlerConfiguration.RequestHandlerFactory.ShouldBeTypeOf<CustomRequestHandlerFactory>();
			}

			[Test]
			public void it_should_autodetect_request_handlers()
			{
				_handlerConfiguration.RequestHandlers.Any(x => x == typeof(TestRequestHandler)).ShouldBe(true);
			}
		}

		public abstract class given_bootstrapper
		{
			protected Bootstrapper Bootstrapper;

			protected given_bootstrapper()
			{
				Bootstrapper = new Bootstrapper(Assembly.GetExecutingAssembly());
			}
		}

		#region Test Classes

		public class StartupTask : IStartupTask
		{
			public void Run(HttpHandlerConfiguration configuration)
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