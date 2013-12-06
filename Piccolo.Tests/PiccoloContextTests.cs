using System.Reflection;
using NUnit.Framework;
using Shouldly;

namespace Piccolo.Tests
{
	public class PiccoloHttpHandlerTests
	{
		[TestFixture]
		public class when_initialised
		{
			private PiccoloHttpHandler _httpHandler;

			[SetUp]
			public void SetUp()
			{
				_httpHandler = new PiccoloHttpHandler(Assembly.GetExecutingAssembly());
			}

			[Test]
			public void it_should_initialise_configuration()
			{
				_httpHandler.Configuration.ShouldNotBe(null);
			}

			[Test]
			public void it_should_initialise_engine()
			{
				_httpHandler.Engine.ShouldNotBe(null);
			}

			[Test]
			public void it_should_be_reusablee_across_requests()
			{
				_httpHandler.IsReusable.ShouldBe(true);
			}
		}
	}
}