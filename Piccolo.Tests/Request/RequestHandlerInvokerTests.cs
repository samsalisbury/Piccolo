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

		[TestFixture]
		public class when_executing_put_request : given_request_handler_invoker
		{
			private string _result;

			[SetUp]
			public void SetUp()
			{
				var routeParameters = new Dictionary<string, string> {{"route", "1"}};
				var contextualParameters = new Dictionary<string, object>{{"context", 2}};
				var queryParameters = new Dictionary<string, string> {{"query", "3"}};
				var rawPayload = "4";

				_result = Invoker.Execute(new PutResource(), "PUT", routeParameters, queryParameters, contextualParameters, rawPayload, null).Content.ReadAsStringAsync().Result;
			}

			[Test]
			public void it_should_bind_parameters()
			{
				_result.ShouldBe("route:1, context:2, query:3, payload:4");
			}

			[ExcludeFromCodeCoverage]
			public class PutResource : IPut<string, string>
			{
				public int Route { get; set; }

				[Contextual]
				public int Context { get; set; }

				[Optional]
				public int Query { get; set; }
				
				public HttpResponseMessage<string> Put(string parameters)
				{
					return new HttpResponseMessage<string>(new HttpResponseMessage { Content = new StringContent(string.Format("route:{0}, context:{1}, query:{2}, payload:{3}", Route, Context, Query, parameters)) });
				}
			}
		}

		[TestFixture]
		public class when_executing_patch_request : given_request_handler_invoker
		{
			private string _result;

			[SetUp]
			public void SetUp()
			{
				var routeParameters = new Dictionary<string, string> {{"route", "1"}};
				var contextualParameters = new Dictionary<string, object>{{"context", 2}};
				var queryParameters = new Dictionary<string, string> {{"query", "3"}};
				var rawPayload = "4";

				_result = Invoker.Execute(new PatchResource(), "PATCH", routeParameters, queryParameters, contextualParameters, rawPayload, null).Content.ReadAsStringAsync().Result;
			}

			[Test]
			public void it_should_bind_parameters()
			{
				_result.ShouldBe("route:1, context:2, query:3, payload:4");
			}

			[ExcludeFromCodeCoverage]
			public class PatchResource : IPatch<string, string>
			{
				public int Route { get; set; }

				[Contextual]
				public int Context { get; set; }

				[Optional]
				public int Query { get; set; }
				
				public HttpResponseMessage<string> Patch(string parameters)
				{
					return new HttpResponseMessage<string>(new HttpResponseMessage { Content = new StringContent(string.Format("route:{0}, context:{1}, query:{2}, payload:{3}", Route, Context, Query, parameters)) });
				}
			}
		}

		[TestFixture]
		public class when_executing_delete_request : given_request_handler_invoker
		{
			private string _result;

			[SetUp]
			public void SetUp()
			{
				var routeParameters = new Dictionary<string, string> {{"route", "1"}};
				var contextualParameters = new Dictionary<string, object>{{"context", 2}};
				var queryParameters = new Dictionary<string, string> {{"query", "3"}};
				var rawPayload = "4";

				_result = Invoker.Execute(new DeleteResource(), "DELETE", routeParameters, queryParameters, contextualParameters, rawPayload, null).Content.ReadAsStringAsync().Result;
			}

			[Test]
			public void it_should_bind_parameters()
			{
				_result.ShouldBe("route:1, context:2, query:3, payload:4");
			}

			[ExcludeFromCodeCoverage]
			public class DeleteResource : IDelete<string, string>
			{
				public int Route { get; set; }

				[Contextual]
				public int Context { get; set; }

				[Optional]
				public int Query { get; set; }
				
				public HttpResponseMessage<string> Delete(string parameters)
				{
					return new HttpResponseMessage<string>(new HttpResponseMessage { Content = new StringContent(string.Format("route:{0}, context:{1}, query:{2}, payload:{3}", Route, Context, Query, parameters)) });
				}
			}
		}

		[TestFixture]
		public class when_executing_request_with_missing_payload : given_request_handler_invoker
		{
			[Test]
			public void it_should_throw_exception()
			{
				const string rawPayload = null;

				Assert.Throws<MissingPayloadException>(() => Invoker.Execute(new PostResource(), "POST", new Dictionary<string, string>(), new Dictionary<string, string>(), new Dictionary<string, object>(), rawPayload, null));
			}

			[ExcludeFromCodeCoverage]
			public class PostResource : IPost<string, string>
			{
				public HttpResponseMessage<string> Post(string parameters)
				{
					throw new Exception();
				}
			}
		}

		[TestFixture]
		public class when_executing_request_with_malformed_payload : given_request_handler_invoker
		{
			[Test]
			public void it_should_throw_exception()
			{
				const string rawPayload = "{";

				Assert.Throws<MalformedPayloadException>(() => Invoker.Execute(new PostResource(), "POST", new Dictionary<string, string>(), new Dictionary<string, string>(), new Dictionary<string, object>(), rawPayload, null));
			}

			[ExcludeFromCodeCoverage]
			public class PostResource : IPost<string, string>
			{
				public HttpResponseMessage<string> Post(string parameters)
				{
					throw new Exception();
				}
			}
		}

		[TestFixture]
		public class when_executing_request_with_malformed_payload_parameter : given_request_handler_invoker
		{
			[Test]
			public void it_should_throw_exception()
			{
				const string rawPayload = "{ dateTime: \"\" }";

				Assert.Throws<MalformedPayloadException>(() => Invoker.Execute(new PostResource(), "POST", new Dictionary<string, string>(), new Dictionary<string, string>(), new Dictionary<string, object>(), rawPayload, null));
			}

			[ExcludeFromCodeCoverage]
			public class PostResource : IPost<Params, string>
			{
				public HttpResponseMessage<string> Post(Params parameters)
				{
					throw new Exception();
				}
			}

			public class Params
			{
				public DateTime DateTime { get; set; }
			}
		}

		[TestFixture]
		public class when_executing_request_with_invalid_payload : given_request_handler_invoker
		{
			private HttpContent _content;

			[SetUp]
			public void SetUp()
			{
				const string rawPayload = "\"payload\"";
				object payloadValidator = new Validator();

				_content = Invoker.Execute(new PostResource(), "POST", new Dictionary<string, string>(), new Dictionary<string, string>(), new Dictionary<string, object>(), rawPayload, payloadValidator).Content;
			}

			[Test]
			public void it_should_bind_parameters()
			{
				((ObjectContent)_content).Content.ToString().ShouldBe("{ error = meh }");
			}

			[ExcludeFromCodeCoverage]
			public class PostResource : IPost<string, string>
			{
				public HttpResponseMessage<string> Post(string parameters)
				{
					throw new Exception();
				}
			}

			public class Validator : IPayloadValidator<string>
			{
				public ValidationResult Validate(string payload)
				{
					return new ValidationResult("meh");
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