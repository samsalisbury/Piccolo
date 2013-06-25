using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using NUnit.Framework;
using Piccolo.Configuration;
using Piccolo.Request.HandlerInvokers;
using Shouldly;

namespace Piccolo.UnitTests.Request.HandlerInvokers
{
	public class DeleteRequestHandlerInvokerTests
	{
		[TestFixture]
		public class when_executing_request : given_delete_request_handler_invoker
		{
			private string _result;

			[SetUp]
			public void SetUp()
			{
				var dictionary = new Dictionary<string, string> {{"Param", "Test"}};
				_result = Invoker.Execute(new DeleteResource(), dictionary).Content.ReadAsStringAsync().Result;
			}

			[Test]
			public void it_should()
			{
				_result.ShouldBe("Test");
			}
		}

		public abstract class given_delete_request_handler_invoker
		{
			protected IRequestHandlerInvoker Invoker;

			protected given_delete_request_handler_invoker()
			{
				var configuration = new Bootstrapper(Assembly.GetExecutingAssembly()).ApplyConfiguration(false);
				Invoker = new DeleteRequestHandlerInvoker(configuration.RouteParameterBinders);
			}
		}

		[Route("/DeleteRequestHandlerInvokerTests/{Param1}")]
		public class DeleteResource : IDelete
		{
			public HttpResponseMessage<dynamic> Delete()
			{
				return new HttpResponseMessage<dynamic>(new HttpResponseMessage {Content = new StringContent(Param)});
			}

			public string Param { get; set; }
		}
	}
}