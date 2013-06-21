using NUnit.Framework;
using Piccolo.IoC;
using Shouldly;

namespace Piccolo.UnitTests.IoC
{
	public class DefaultRequestHandlerFactoryTests
	{
		[TestFixture]
		public class when_creating_instance : given_default_request_handler_factory
		{
			[Test]
			public void it_should_return_instance()
			{
				HandlerFactory.CreateInstance(typeof(TestRequestHandler)).ShouldBeTypeOf<TestRequestHandler>();
			}
		}

		public abstract class given_default_request_handler_factory
		{
			protected DefaultRequestHandlerFactory HandlerFactory;

			protected given_default_request_handler_factory()
			{
				HandlerFactory = new DefaultRequestHandlerFactory();
			}
		}
	}
}