using System.Collections.Generic;
using NUnit.Framework;
using Piccolo.Request.HandlerInvokers;
using Shouldly;

namespace Piccolo.UnitTests.Request.HandlerInvokers
{
	public class GetRequestHandlerInvokerTests
	{
		[TestFixture]
		public class when_executing_request : given_get_request_handler_invoker
		{
			private string _result;

			[SetUp]
			public void SetUp()
			{
				var dictionary = new Dictionary<string, string>
					{
						{"Param1", "Test"},
						{"Param2", "1"}
					};
				_result = Invoker.Execute(new GetResourceById(), dictionary).Content.ReadAsStringAsync().Result;
			}

			[Test]
			public void it_should()
			{
				_result.ShouldBe("Test=1");
			}
		}

		public abstract class given_get_request_handler_invoker
		{
			protected IRequestHandlerInvoker Invoker;

			protected given_get_request_handler_invoker()
			{
				Invoker = new GetRequestHandlerInvoker();
			}
		}

		[Route("/GetRequestHandlerInvokerTests/{Param1}/{Param2}")]
		public class GetResourceById : IGet<string>
		{
			public HttpResponseMessage<string> Get()
			{
				return Response.Success.Ok(string.Format("{0}={1}", Param1, Param2));
			}

			public string Param1 { get; set; }
			public int Param2 { get; set; }
		}
	}
}