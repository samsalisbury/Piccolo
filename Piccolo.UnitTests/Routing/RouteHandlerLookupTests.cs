using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
			private RouteHandlerLookupResult _routeHandlerLookupResult;

			[SetUp]
			public void SetUp()
			{
				_routeHandlerLookupResult = RouteHandlerLookup.FindRequestHandler("get", "/");
			}

			[Test]
			public void it_should_return_request_handler()
			{
				_routeHandlerLookupResult.RequestHandlerType.ShouldBe(typeof(RootRequestHandler));
			}
		}

		[TestFixture]
		public class when_searching_for_request_handler_that_matches_static_level1_path : given_route_handler_lookup_initialised_with_test_routes
		{
			private RouteHandlerLookupResult _routeHandlerLookupResult;

			[SetUp]
			public void SetUp()
			{
				_routeHandlerLookupResult = RouteHandlerLookup.FindRequestHandler("get", "/level-1");
			}

			[Test]
			public void it_should_return_request_handler()
			{
				_routeHandlerLookupResult.RequestHandlerType.ShouldBe(typeof(StaticLevel1RequestHandler));
			}
		}

		[TestFixture]
		public class when_searching_for_request_handler_that_matches_static_level2_path : given_route_handler_lookup_initialised_with_test_routes
		{
			private RouteHandlerLookupResult _routeHandlerLookupResult;

			[SetUp]
			public void SetUp()
			{
				_routeHandlerLookupResult = RouteHandlerLookup.FindRequestHandler("get", "/level-1/level-2");
			}

			[Test]
			public void it_should_return_request_handler()
			{
				_routeHandlerLookupResult.RequestHandlerType.ShouldBe(typeof(StaticLevel2RequestHandler));
			}
		}

		[TestFixture]
		public class when_searching_for_request_handler_that_matches_alternative_path : given_route_handler_lookup_initialised_with_test_routes
		{
			private RouteHandlerLookupResult _routeHandlerLookupResult;

			[SetUp]
			public void SetUp()
			{
				_routeHandlerLookupResult = RouteHandlerLookup.FindRequestHandler("get", "/alternative-path");
			}

			[Test]
			public void it_should_return_request_handler()
			{
				_routeHandlerLookupResult.RequestHandlerType.ShouldBe(typeof(StaticLevel2RequestHandler));
			}
		}

		[TestFixture]
		public class when_searching_for_request_handler_that_matches_dynamic_level1_path_with_int32_property : given_route_handler_lookup_initialised_with_test_routes
		{
			private RouteHandlerLookupResult _routeHandlerLookupResult;

			[SetUp]
			public void SetUp()
			{
				_routeHandlerLookupResult = RouteHandlerLookup.FindRequestHandler("get", "/DynamicLevel1Int32/1");
			}

			[Test]
			public void it_should_return_request_handler()
			{
				_routeHandlerLookupResult.RequestHandlerType.ShouldBe(typeof(DynamicLevel1Int32RequestHandler));
			}

			[Test]
			public void it_should_return_parameter()
			{
				_routeHandlerLookupResult.RouteParameters.Any(pair => pair.Key == "value" && pair.Value == "1").ShouldBe(true);
			}
		}

		[TestFixture]
		public class when_searching_for_request_handler_that_matches_dynamic_level1_path_with_int64_property : given_route_handler_lookup_initialised_with_test_routes
		{
			private RouteHandlerLookupResult _routeHandlerLookupResult;

			[SetUp]
			public void SetUp()
			{
				_routeHandlerLookupResult = RouteHandlerLookup.FindRequestHandler("get", "/DynamicLevel1Int64/1");
			}

			[Test]
			public void it_should_return_request_handler()
			{
				_routeHandlerLookupResult.RequestHandlerType.ShouldBe(typeof(DynamicLevel1Int64RequestHandler));
			}

			[Test]
			public void it_should_return_parameter()
			{
				_routeHandlerLookupResult.RouteParameters.Any(pair => pair.Key == "value" && pair.Value == "1").ShouldBe(true);
			}
		}

		[TestFixture]
		public class when_searching_for_request_handler_that_matches_dynamic_level1_path_with_string_property : given_route_handler_lookup_initialised_with_test_routes
		{
			private RouteHandlerLookupResult _routeHandlerLookupResult;

			[SetUp]
			public void SetUp()
			{
				_routeHandlerLookupResult = RouteHandlerLookup.FindRequestHandler("get", "/DynamicLevel1String/text");
			}

			[Test]
			public void it_should_return_request_handler()
			{
				_routeHandlerLookupResult.RequestHandlerType.ShouldBe(typeof(DynamicLevel1StringRequestHandler));
			}

			[Test]
			public void it_should_return_parameter()
			{
				_routeHandlerLookupResult.RouteParameters.Any(pair => pair.Key == "value" && pair.Value == "text").ShouldBe(true);
			}
		}

		[TestFixture]
		public class when_searching_for_request_handler_that_matches_dynamic_level2_path : given_route_handler_lookup_initialised_with_test_routes
		{
			private RouteHandlerLookupResult _routeHandlerLookupResult;

			[SetUp]
			public void SetUp()
			{
				_routeHandlerLookupResult = RouteHandlerLookup.FindRequestHandler("get", "/level-1/2");
			}

			[Test]
			public void it_should_return_request_handler()
			{
				_routeHandlerLookupResult.RequestHandlerType.ShouldBe(typeof(DynamicLevel2RequestHandler));
			}
		}

		[TestFixture]
		public class when_searching_for_request_handler_that_matches_dynamic_multi_level_path : given_route_handler_lookup_initialised_with_test_routes
		{
			private RouteHandlerLookupResult _routeHandlerLookupResult;

			[SetUp]
			public void SetUp()
			{
				_routeHandlerLookupResult = RouteHandlerLookup.FindRequestHandler("get", "/1/2/3");
			}

			[Test]
			public void it_should_return_request_handler()
			{
				_routeHandlerLookupResult.RequestHandlerType.ShouldBe(typeof(DynamicMultiLevelRequestHandler));
			}
		}

		[TestFixture]
		public class when_searching_for_request_handler_that_matches_dynamic_multi_level_path_that_is_not_routed : given_route_handler_lookup_initialised_with_test_routes
		{
			private RouteHandlerLookupResult _routeHandlerLookupResult;

			[SetUp]
			public void SetUp()
			{
				_routeHandlerLookupResult = RouteHandlerLookup.FindRequestHandler("get", "/1/2/3/is-not-routed");
			}

			[Test]
			public void it_should_return_null()
			{
				_routeHandlerLookupResult.ShouldBe(null);
			}
		}

		[TestFixture]
		public class when_searching_for_request_handler_that_does_not_match_parameter_name : given_route_handler_lookup_initialised_with_test_routes
		{
			private RouteHandlerLookupResult _routeHandlerLookupResult;

			[SetUp]
			public void SetUp()
			{
				_routeHandlerLookupResult = RouteHandlerLookup.FindRequestHandler("get", "/type-mismatch/1");
			}

			[Test]
			public void it_should_return_null()
			{
				_routeHandlerLookupResult.RequestHandlerType.ShouldBe(null);
			}
		}

		[TestFixture]
		public class when_searching_for_request_handler : given_route_handler_lookup_initialised_with_0_routes
		{
			private RouteHandlerLookupResult _routeHandlerLookupResult;

			[SetUp]
			public void SetUp()
			{
				_routeHandlerLookupResult = RouteHandlerLookup.FindRequestHandler(string.Empty, string.Empty);
			}

			[Test]
			public void it_should_return_null()
			{
				_routeHandlerLookupResult.ShouldBe(null);
			}
		}

		public abstract class given_route_handler_lookup_initialised_with_test_routes : given_route_handler_lookup
		{
			private static readonly IEnumerable<Type> _testRoutes = new List<Type>
				{
					typeof(RootRequestHandler),
					typeof(StaticLevel1RequestHandler),
					typeof(StaticLevel2RequestHandler),
					typeof(DynamicLevel1Int32RequestHandler),
					typeof(DynamicLevel1Int64RequestHandler),
					typeof(DynamicLevel1StringRequestHandler),
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

		[Route("/DynamicLevel1Int32/{Value}")]
		public class DynamicLevel1Int32RequestHandler : IGet<string>
		{
			[ExcludeFromCodeCoverage]
			public HttpResponseMessage<string> Get()
			{
				return new HttpResponseMessage<string>(new HttpResponseMessage());
			}

			public Int32 Value { get; set; }
		}

		[Route("/DynamicLevel1Int64/{Value}")]
		public class DynamicLevel1Int64RequestHandler : IGet<string>
		{
			[ExcludeFromCodeCoverage]
			public HttpResponseMessage<string> Get()
			{
				return new HttpResponseMessage<string>(new HttpResponseMessage());
			}

			public Int64 Value { get; set; }
		}

		[Route("/DynamicLevel1String/{Value}")]
		public class DynamicLevel1StringRequestHandler : IGet<string>
		{
			[ExcludeFromCodeCoverage]
			public HttpResponseMessage<string> Get()
			{
				return new HttpResponseMessage<string>(new HttpResponseMessage());
			}

			public string Value { get; set; }
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