using System.Collections.Generic;
using System.Net.Http;
using NUnit.Framework;
using Piccolo.Request.HandlerInvokers;
using Shouldly;

namespace Piccolo.UnitTests.Request.HandlerInvokers
{
	public class PutRequestHandlerInvokerTests
	{
		[TestFixture]
		public class when_executing_request : given_put_request_handler_invoker
		{
			private string _result;

			[SetUp]
			public void SetUp()
			{
				var dictionary = new Dictionary<string, string> {{"Param", "Test"}};
				_result = Invoker.Execute(new PutResource(), dictionary).Content.ReadAsStringAsync().Result;
			}

			[Test]
			public void it_should()
			{
				_result.ShouldBe("Test");
			}
		}

		public abstract class given_put_request_handler_invoker
		{
			protected IRequestHandlerInvoker Invoker;

			protected given_put_request_handler_invoker()
			{
				Invoker = new PutRequestHandlerInvoker();
			}
		}

		[Route("/PutRequestHandlerInvokerTests/{Param1}")]
		public class PutResource : IPut<string>
		{
			public HttpResponseMessage<dynamic> Put(string parameters)
			{
				return new HttpResponseMessage<dynamic>(new HttpResponseMessage {Content = new StringContent(Param)});
			}

			public string Param { get; set; }
		}
	}
}