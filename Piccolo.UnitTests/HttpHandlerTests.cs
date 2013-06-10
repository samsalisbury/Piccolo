using System;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using Piccolo.Configuration;
using Piccolo.IoC;
using Shouldly;

namespace Piccolo.UnitTests
{
	[TestFixture]
	public class HttpHandlerTests
	{
		public class when_instantiated_with_default_configuration : given_http_handler_with_default_configuration
		{
			[Test]
			public void it_should_configure_default_request_handler_factory()
			{
				HttpHandler.Configuration.RequestHandlerFactory.ShouldBeTypeOf<DefaultRequestHandlerFactory>();
			}
		}

		public class when_instantiated : given_http_handler
		{
			[Test]
			public void it_should_configure_custom_request_handler_factory()
			{
				HttpHandler.Configuration.RequestHandlerFactory.ShouldBeTypeOf<CustomRequestHandlerFactory>();
			}
		}

		public abstract class given_http_handler_with_default_configuration : given_http_handler
		{
			protected given_http_handler_with_default_configuration() : base(false)
			{
			}
		}

		public abstract class given_http_handler
		{
			protected HttpHandler HttpHandler;

			protected given_http_handler() : this(true)
			{
			}

			protected given_http_handler(bool applyCustomConfiguration)
			{
				HttpHandler = new HttpHandler(applyCustomConfiguration);
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