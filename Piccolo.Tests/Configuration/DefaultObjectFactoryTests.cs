using NUnit.Framework;
using Piccolo.Configuration;
using Shouldly;

namespace Piccolo.Tests.Configuration
{
	public class DefaultObjectFactoryTests
	{
		[TestFixture]
		public class when_creating_instance : given_default_object_factory
		{
			[Test]
			public void it_should_return_instance()
			{
				HandlerFactory.CreateInstance<object>(typeof(object)).ShouldBeTypeOf<object>();
			}
		}

		public abstract class given_default_object_factory
		{
			protected DefaultObjectFactory HandlerFactory;

			protected given_default_object_factory()
			{
				HandlerFactory = new DefaultObjectFactory();
			}
		}
	}
}