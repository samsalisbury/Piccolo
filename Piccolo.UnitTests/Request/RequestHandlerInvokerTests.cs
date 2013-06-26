using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using NUnit.Framework;
using Piccolo.Configuration;
using Piccolo.Request;
using Shouldly;

namespace Piccolo.UnitTests.Request
{
	public class RequestHandlerInvokerTests
	{
		[TestFixture]
		public class when_executing_get_request_with_0_parameters : given_request_handler_invoker
		{
			private string _result;

			[SetUp]
			public void SetUp()
			{
				_result = Invoker.Execute(new GetResource(), "GET", new Dictionary<string, string>()).Content.ReadAsStringAsync().Result;
			}

			[Test]
			public void it_should()
			{
				_result.ShouldBe("TEST");
			}
		}

		[TestFixture]
		public class when_executing_get_request_with_string_parameter : given_request_handler_invoker
		{
			private string _result;

			[SetUp]
			public void SetUp()
			{
				var routeParameters = new Dictionary<string, string> { { "param", "TEST" } };
				_result = Invoker.Execute(new GetResourceString(), "GET", routeParameters).Content.ReadAsStringAsync().Result;
			}

			[Test]
			public void it_should()
			{
				_result.ShouldBe("GET TEST");
			}
		}

		[TestFixture]
		public class when_executing_get_request_with_int16_parameter : given_request_handler_invoker
		{
			private string _result;

			[SetUp]
			public void SetUp()
			{
				var routeParameters = new Dictionary<string, string> {{"param", "1"}};
				_result = Invoker.Execute(new GetResourceInt16(), "GET", routeParameters).Content.ReadAsStringAsync().Result;
			}

			[Test]
			public void it_should()
			{
				_result.ShouldBe("GET 1");
			}
		}

		[TestFixture]
		public class when_executing_get_request_with_int32_parameter : given_request_handler_invoker
		{
			private string _result;

			[SetUp]
			public void SetUp()
			{
				var routeParameters = new Dictionary<string, string> {{"param", "1"}};
				_result = Invoker.Execute(new GetResourceInt32(), "GET", routeParameters).Content.ReadAsStringAsync().Result;
			}

			[Test]
			public void it_should()
			{
				_result.ShouldBe("GET 1");
			}
		}

		[TestFixture]
		public class when_executing_put_request : given_request_handler_invoker
		{
			private string _result;

			[SetUp]
			public void SetUp()
			{
				var routeParameters = new Dictionary<string, string> {{"param", "put"}};
				_result = Invoker.Execute(new PutResource(), "PUT", routeParameters).Content.ReadAsStringAsync().Result;
			}

			[Test]
			public void it_should()
			{
				_result.ShouldBe("put");
			}
		}

		[TestFixture]
		public class when_executing_post_request : given_request_handler_invoker
		{
			private string _result;

			[SetUp]
			public void SetUp()
			{
				var routeParameters = new Dictionary<string, string> {{"param", "post"}};
				_result = Invoker.Execute(new PostResource(), "POST", routeParameters).Content.ReadAsStringAsync().Result;
			}

			[Test]
			public void it_should()
			{
				_result.ShouldBe("post");
			}
		}

		[TestFixture]
		public class when_executing_delete_request : given_request_handler_invoker
		{
			private string _result;

			[SetUp]
			public void SetUp()
			{
				var routeParameters = new Dictionary<string, string> {{"param", "delete"}};
				_result = Invoker.Execute(new DeleteResource(), "DELETE", routeParameters).Content.ReadAsStringAsync().Result;
			}

			[Test]
			public void it_should()
			{
				_result.ShouldBe("delete");
			}
		}

		public abstract class given_request_handler_invoker
		{
			protected RequestHandlerInvoker Invoker;

			protected given_request_handler_invoker()
			{
				var configuration = new Bootstrapper(Assembly.GetExecutingAssembly()).ApplyConfiguration(false);
				Invoker = new RequestHandlerInvoker(configuration.RouteParameterBinders);
			}
		}

		[Route("/RequestHandlerInvokerTests")]
		public class GetResource : IGet<string>
		{
			public HttpResponseMessage<string> Get()
			{
				return Response.Success.Ok("TEST");
			}
		}

		[Route("/RequestHandlerInvokerTests/String/{Param}")]
		public class GetResourceString : IGet<string>
		{
			public HttpResponseMessage<string> Get()
			{
				return Response.Success.Ok(string.Format("GET {0}", Param));
			}

			public String Param { get; set; }
		}

		[Route("/RequestHandlerInvokerTests/Int16/{Param}")]
		public class GetResourceInt16 : IGet<string>
		{
			public HttpResponseMessage<string> Get()
			{
				return Response.Success.Ok(string.Format("GET {0}", Param));
			}

			public Int16 Param { get; set; }
		}

		[Route("/RequestHandlerInvokerTests/Int32/{Param}")]
		public class GetResourceInt32 : IGet<string>
		{
			public HttpResponseMessage<string> Get()
			{
				return Response.Success.Ok(string.Format("GET {0}", Param));
			}

			public Int32 Param { get; set; }
		}

		[Route("/PutRequestHandlerInvokerTests/{Param}")]
		public class PutResource : IPut<string>
		{
			public HttpResponseMessage<dynamic> Put(string parameters)
			{
				return new HttpResponseMessage<dynamic>(new HttpResponseMessage {Content = new StringContent(Param)});
			}

			public string Param { get; set; }
		}

		[Route("/PostRequestHandlerInvokerTests/{Param}")]
		public class PostResource : IPost<string>
		{
			public HttpResponseMessage<dynamic> Post(string parameters)
			{
				return new HttpResponseMessage<dynamic>(new HttpResponseMessage {Content = new StringContent(Param)});
			}

			public string Param { get; set; }
		}

		[Route("/DeleteRequestHandlerInvokerTests/{Param}")]
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