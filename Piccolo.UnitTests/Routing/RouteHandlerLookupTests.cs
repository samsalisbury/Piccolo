using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using NUnit.Framework;
using Piccolo.Routing;
using Shouldly;

namespace Piccolo.UnitTests.Routing
{
	public class RouteHandlerLookupTests
	{
		[TestFixture]
		public class when_searching_for_request_handler_that_matches_root : given_route_handler_lookup_initialised_with_test_routes
		{
			private Type _requestHandler;

			[SetUp]
			public void SetUp()
			{
				_requestHandler = RouteHandlerLookup.FindRequestHandler("get", "/");
			}

			[Test]
			public void it_should_return_request_handler()
			{
				_requestHandler.ShouldBe(typeof(RootRequestHandler));
			}
		}

		[TestFixture]
		public class when_searching_for_request_handler_that_matches_static_level1_path : given_route_handler_lookup_initialised_with_test_routes
		{
			private Type _requestHandler;

			[SetUp]
			public void SetUp()
			{
				_requestHandler = RouteHandlerLookup.FindRequestHandler("get", "/level-1");
			}

			[Test]
			public void it_should_return_request_handler()
			{
				_requestHandler.ShouldBe(typeof(StaticLevel1RequestHandler));
			}
		}

		[TestFixture]
		public class when_searching_for_request_handler_that_matches_static_level2_path : given_route_handler_lookup_initialised_with_test_routes
		{
			private Type _requestHandler;

			[SetUp]
			public void SetUp()
			{
				_requestHandler = RouteHandlerLookup.FindRequestHandler("get", "/level-1/level-2");
			}

			[Test]
			public void it_should_return_request_handler()
			{
				_requestHandler.ShouldBe(typeof(StaticLevel2RequestHandler));
			}
		}

		[TestFixture]
		public class when_searching_for_request_handler_that_matches_alternative_path : given_route_handler_lookup_initialised_with_test_routes
		{
			private Type _requestHandler;

			[SetUp]
			public void SetUp()
			{
				_requestHandler = RouteHandlerLookup.FindRequestHandler("get", "/alternative-path");
			}

			[Test]
			public void it_should_return_request_handler()
			{
				_requestHandler.ShouldBe(typeof(StaticLevel2RequestHandler));
			}
		}

		[TestFixture]
		public class when_searching_for_request_handler_that_matches_dynamic_level1_path : given_route_handler_lookup_initialised_with_test_routes
		{
			private Type _requestHandler;

			[SetUp]
			public void SetUp()
			{
				_requestHandler = RouteHandlerLookup.FindRequestHandler("get", "/1");
			}

			[Test]
			public void it_should_return_request_handler()
			{
				_requestHandler.ShouldBe(typeof(DynamicLevel1RequestHandler));
			}
		}

		[TestFixture]
		public class when_searching_for_request_handler_that_matches_dynamic_level2_path : given_route_handler_lookup_initialised_with_test_routes
		{
			private Type _requestHandler;

			[SetUp]
			public void SetUp()
			{
				_requestHandler = RouteHandlerLookup.FindRequestHandler("get", "/level-1/2");
			}

			[Test]
			public void it_should_return_request_handler()
			{
				_requestHandler.ShouldBe(typeof(DynamicLevel2RequestHandler));
			}
		}

		[TestFixture]
		public class when_searching_for_request_handler_that_matches_dynamic_multi_level_path : given_route_handler_lookup_initialised_with_test_routes
		{
			private Type _requestHandler;

			[SetUp]
			public void SetUp()
			{
				_requestHandler = RouteHandlerLookup.FindRequestHandler("get", "/1/2/3");
			}

			[Test]
			public void it_should_return_request_handler()
			{
				_requestHandler.ShouldBe(typeof(DynamicMultiLevelRequestHandler));
			}
		}

		[TestFixture]
		public class when_searching_for_request_handler_that_matches_dynamic_multi_level_path_that_is_not_routed : given_route_handler_lookup_initialised_with_test_routes
		{
			private Type _requestHandler;

			[SetUp]
			public void SetUp()
			{
				_requestHandler = RouteHandlerLookup.FindRequestHandler("get", "/1/2/3/is-not-routed");
			}

			[Test]
			public void it_should_return_null()
			{
				_requestHandler.ShouldBe(null);
			}
		}

		[TestFixture]
		public class when_searching_for_request_handler_that_does_not_match_parameter_name : given_route_handler_lookup_initialised_with_test_routes
		{
			private Type _requestHandler;

			[SetUp]
			public void SetUp()
			{
				_requestHandler = RouteHandlerLookup.FindRequestHandler("get", "/type-mismatch/1");
			}

			[Test]
			public void it_should_return_null()
			{
				_requestHandler.ShouldBe(null);
			}
		}

		[TestFixture]
		public class when_searching_for_request_handler : given_route_handler_lookup_initialised_with_0_routes
		{
			private Type _requestHandler;

			[SetUp]
			public void SetUp()
			{
				_requestHandler = RouteHandlerLookup.FindRequestHandler(string.Empty, string.Empty);
			}

			[Test]
			public void it_should_return_null()
			{
				_requestHandler.ShouldBe(null);
			}
		}

		public abstract class given_route_handler_lookup_initialised_with_test_routes : given_route_handler_lookup
		{
			private static readonly IEnumerable<Type> _testRoutes = new List<Type>
				{
					typeof(RootRequestHandler),
					typeof(StaticLevel1RequestHandler),
					typeof(StaticLevel2RequestHandler),
					typeof(DynamicLevel1RequestHandler),
					typeof(DynamicLevel1RequestHandlerWithInputParameterNameMismatch),
					typeof(DynamicLevel2RequestHandler),
					typeof(DynamicMultiLevelRequestHandler)
				};

			protected given_route_handler_lookup_initialised_with_test_routes() : base(_testRoutes)
			{
			}
		}

		public abstract class given_route_handler_lookup_initialised_with_0_routes : given_route_handler_lookup
		{
			protected given_route_handler_lookup_initialised_with_0_routes() : base(new List<Type>())
			{
			}
		}

		public abstract class given_route_handler_lookup
		{
			protected RouteHandlerLookup RouteHandlerLookup;

			protected given_route_handler_lookup(IEnumerable<Type> requestHandlers)
			{
				RouteHandlerLookup = new RouteHandlerLookup(requestHandlers);
			}
		}

		#region Test Classes

		[Route("/")]
		public class RootRequestHandler : IGet<string>
		{
			[ExcludeFromCodeCoverage]
			public HttpResponseMessage<string> Get()
			{
				return new HttpResponseMessage<string>(new HttpResponseMessage());
			}
		}

		[Route("/level-1")]
		public class StaticLevel1RequestHandler : IGet<string>
		{
			[ExcludeFromCodeCoverage]
			public HttpResponseMessage<string> Get()
			{
				return new HttpResponseMessage<string>(new HttpResponseMessage());
			}
		}

		[Route("/level-1/level-2")]
		[Route("/alternative-path")]
		public class StaticLevel2RequestHandler : IGet<string>
		{
			[ExcludeFromCodeCoverage]
			public HttpResponseMessage<string> Get()
			{
				return new HttpResponseMessage<string>(new HttpResponseMessage());
			}
		}

		[Route("/{DynamicLevel1}")]
		public class DynamicLevel1RequestHandler : IGet<string>
		{
			[ExcludeFromCodeCoverage]
			public HttpResponseMessage<string> Get()
			{
				return new HttpResponseMessage<string>(new HttpResponseMessage());
			}

			public int DynamicLevel1 { get; set; }
		}

		[Route("/type-mismatch/{DynamicLevel1}")]
		public class DynamicLevel1RequestHandlerWithInputParameterNameMismatch : IGet<string>
		{
			[ExcludeFromCodeCoverage]
			public HttpResponseMessage<string> Get()
			{
				return new HttpResponseMessage<string>(new HttpResponseMessage());
			}

			public string NameDoesNotMatch { get; set; }
		}

		[Route("/level-1/{DynamicLevel2}")]
		public class DynamicLevel2RequestHandler : IGet<string>
		{
			[ExcludeFromCodeCoverage]
			public HttpResponseMessage<string> Get()
			{
				return new HttpResponseMessage<string>(new HttpResponseMessage());
			}

			public int DynamicLevel2 { get; set; }
		}

		[Route("/{DynamicLevel1}/{DynamicLevel2}/{DynamicLevel3}")]
		public class DynamicMultiLevelRequestHandler : IGet<string>
		{
			[ExcludeFromCodeCoverage]
			public HttpResponseMessage<string> Get()
			{
				return new HttpResponseMessage<string>(new HttpResponseMessage());
			}

			public int DynamicLevel1 { get; set; }
			public int DynamicLevel2 { get; set; }
			public int DynamicLevel3 { get; set; }
		}

		#endregion
	}
}