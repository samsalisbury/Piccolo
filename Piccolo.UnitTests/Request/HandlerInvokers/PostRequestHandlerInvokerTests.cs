using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using NUnit.Framework;
using Piccolo.Configuration;
using Piccolo.Request.HandlerInvokers;
using Shouldly;

namespace Piccolo.UnitTests.Request.HandlerInvokers
{
	public class PostRequestHandlerInvokerTests
	{
		[TestFixture]
		public class when_executing_request : given_post_request_handler_invoker
		{
			private string _result;

			[SetUp]
			public void SetUp()
			{
				var dictionary = new Dictionary<string, string> {{"Param", "Test"}};
				_result = Invoker.Execute(new PostResource(), dictionary).Content.ReadAsStringAsync().Result;
			}

			[Test]
			public void it_should()
			{
				_result.ShouldBe("Test");
			}
		}

		public abstract class given_post_request_handler_invoker
		{
			protected IRequestHandlerInvoker Invoker;

			protected given_post_request_handler_invoker()
			{
				var configuration = new Bootstrapper(Assembly.GetExecutingAssembly()).ApplyConfiguration(false);
				Invoker = new PostRequestHandlerInvoker(configuration.RouteParameterBinders);
			}
		}

		[Route("/PostRequestHandlerInvokerTests/{Param1}")]
		public class PostResource : IPost<string>
		{
			public HttpResponseMessage<dynamic> Post(string parameters)
			{
				return new HttpResponseMessage<dynamic>(new HttpResponseMessage {Content = new StringContent(Param)});
			}

			public string Param { get; set; }
		}
	}
}