using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web;
using NUnit.Framework;
using Piccolo.Configuration;
using Piccolo.Request;
using Piccolo.Validation;
using Shouldly;

namespace Piccolo.Tests.Request
{
	public class RequestHandlerInvokerTests
	{
		[TestFixture]
		public class when_executing_get_request : given_request_handler_invoker
		{
			private string _result;

			[SetUp]
			public void SetUp()
			{
				var routeParameters = new Dictionary<string, string> {{"route", "1"}};
				var contextualParameters = new Dictionary<string, object>{{"context", 2}};
				var queryParameters = new Dictionary<string, string> {{"query", "3"}};

				_result = Invoker.Execute(new GetResource(), "GET", routeParameters, queryParameters, contextualParameters, string.Empty, null).Content.ReadAsStringAsync().Result;
			}

			[Test]
			public void it_should_bind_parameters()
			{
				_result.ShouldBe("route:1, context:2, query:3");
			}

			[ExcludeFromCodeCoverage]
			public class GetResource : IGet<string>
			{
				public int Route { get; set; }

				[Contextual]
				public int Context { get; set; }

				[Optional]
				public int Query { get; set; }

				public HttpResponseMessage<string> Get()
				{
					return new HttpResponseMessage<string>(new HttpResponseMessage {Content = new StringContent(string.Format("route:{0}, context:{1}, query:{2}", Route, Context, Query))});
				}
			}
		}

		[TestFixture]
		public class when_executing_post_request : given_request_handler_invoker
		{
			private string _result;

			[SetUp]
			public void SetUp()
			{
				var routeParameters = new Dictionary<string, string> {{"route", "1"}};
				var contextualParameters = new Dictionary<string, object>{{"context", 2}};
				var queryParameters = new Dictionary<string, string> {{"query", "3"}};
				var rawPayload = "4";

				_result = Invoker.Execute(new PostResource(), "POST", routeParameters, queryParameters, contextualParameters, rawPayload, null).Content.ReadAsStringAsync().Result;
			}

			[Test]
			public void it_should_bind_parameters()
			{
				_result.ShouldBe("route:1, context:2, query:3, payload:4");
			}

			[ExcludeFromCodeCoverage]
			public class PostResource : IPost<string, string>
			{
				public int Route { get; set; }

				[Contextual]
				public int Context { get; set; }

				[Optional]
				public int Query { get; set; }
				
				public HttpResponseMessage<string> Post(string parameters)
				{
					return new HttpResponseMessage<string>(new HttpResponseMessage { Content = new StringContent(string.Format("route:{0}, context:{1}, query:{2}, payload:{3}", Route, Context, Query, parameters)) });
				}
			}
		}

		public abstract class given_request_handler_invoker
		{
			protected RequestHandlerInvoker Invoker;

			protected given_request_handler_invoker()
			{
				var configuration = Bootstrapper.ApplyConfiguration(Assembly.GetCallingAssembly(), false);
				Invoker = new RequestHandlerInvoker(configuration.JsonDeserialiser, configuration.Parsers);
			}
		}
	}
}