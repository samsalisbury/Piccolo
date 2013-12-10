using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using NUnit.Framework;
using Piccolo.Routing;
using Shouldly;

namespace Piccolo.Tests.Routing
{
	public class RequestRouterTests
	{
		#region Static/Dynamic Routing

		[TestFixture]
		public class when_searching_for_request_handler_within_virtual_application : given_request_router_initialised_with_test_routes
		{
			private RouteHandlerLookupResult _routeHandlerLookupResult;

			[SetUp]
			public void SetUp()
			{
				_routeHandlerLookupResult = RequestRouter.FindRequestHandler("get", "/v1", new Uri("http://test.com/v1", UriKind.Absolute));
			}

			[Test]
			public void it_should_return_success()
			{
				_routeHandlerLookupResult.IsSuccessful.ShouldBe(true);
			}

			[Test]
			public void it_should_return_request_handler()
			{
				_routeHandlerLookupResult.RequestHandlerType.ShouldBe(typeof(RootRequestHandler));
			}
		}

		[TestFixture]
		public class when_searching_for_request_handler_that_matches_root : given_request_router_initialised_with_test_routes
		{
			private RouteHandlerLookupResult _routeHandlerLookupResult;

			[SetUp]
			public void SetUp()
			{
				_routeHandlerLookupResult = RequestRouter.FindRequestHandler("get", string.Empty, new Uri("http://test.com/", UriKind.Absolute));
			}

			[Test]
			public void it_should_return_success()
			{
				_routeHandlerLookupResult.IsSuccessful.ShouldBe(true);
			}

			[Test]
			public void it_should_return_request_handler()
			{
				_routeHandlerLookupResult.RequestHandlerType.ShouldBe(typeof(RootRequestHandler));
			}
		}

		[TestFixture]
		public class when_searching_for_request_handler_that_matches_static_level1_path : given_request_router_initialised_with_test_routes
		{
			private RouteHandlerLookupResult _routeHandlerLookupResult;

			[SetUp]
			public void SetUp()
			{
				_routeHandlerLookupResult = RequestRouter.FindRequestHandler("get", string.Empty, new Uri("http://test.com/level-1", UriKind.Absolute));
			}

			[Test]
			public void it_should_return_success()
			{
				_routeHandlerLookupResult.IsSuccessful.ShouldBe(true);
			}

			[Test]
			public void it_should_return_request_handler()
			{
				_routeHandlerLookupResult.RequestHandlerType.ShouldBe(typeof(StaticLevel1RequestHandler));
			}
		}

		[TestFixture]
		public class when_searching_for_request_handler_that_matches_static_level2_path : given_request_router_initialised_with_test_routes
		{
			private RouteHandlerLookupResult _routeHandlerLookupResult;

			[SetUp]
			public void SetUp()
			{
				_routeHandlerLookupResult = RequestRouter.FindRequestHandler("get", string.Empty, new Uri("http://test.com/level-1/level-2", UriKind.Absolute));
			}

			[Test]
			public void it_should_return_success()
			{
				_routeHandlerLookupResult.IsSuccessful.ShouldBe(true);
			}

			[Test]
			public void it_should_return_request_handler()
			{
				_routeHandlerLookupResult.RequestHandlerType.ShouldBe(typeof(StaticLevel2RequestHandler));
			}
		}

		[TestFixture]
		public class when_searching_for_request_handler_that_matches_alternative_path : given_request_router_initialised_with_test_routes
		{
			private RouteHandlerLookupResult _routeHandlerLookupResult;

			[SetUp]
			public void SetUp()
			{
				_routeHandlerLookupResult = RequestRouter.FindRequestHandler("get", string.Empty, new Uri("http://test.com/alternative-path", UriKind.Absolute));
			}

			[Test]
			public void it_should_return_success()
			{
				_routeHandlerLookupResult.IsSuccessful.ShouldBe(true);
			}

			[Test]
			public void it_should_return_request_handler()
			{
				_routeHandlerLookupResult.RequestHandlerType.ShouldBe(typeof(StaticLevel2RequestHandler));
			}
		}

		[TestFixture]
		public class when_searching_for_request_handler_that_matches_dynamic_level1_path : given_request_router_initialised_with_test_routes
		{
			private RouteHandlerLookupResult _routeHandlerLookupResult;

			[SetUp]
			public void SetUp()
			{
				_routeHandlerLookupResult = RequestRouter.FindRequestHandler("get", string.Empty, new Uri("http://test.com/1", UriKind.Absolute));
			}

			[Test]
			public void it_should_return_success()
			{
				_routeHandlerLookupResult.IsSuccessful.ShouldBe(true);
			}

			[Test]
			public void it_should_return_request_handler()
			{
				_routeHandlerLookupResult.RequestHandlerType.ShouldBe(typeof(DynamicLevel1RequestHandler));
			}

			[Test]
			public void it_should_bind_route_parameters()
			{
				_routeHandlerLookupResult.RouteParameters.Count.ShouldBe(1);
			}
		}

		[TestFixture]
		public class when_searching_for_request_handler_that_matches_dynamic_level2_path : given_request_router_initialised_with_test_routes
		{
			private RouteHandlerLookupResult _routeHandlerLookupResult;

			[SetUp]
			public void SetUp()
			{
				_routeHandlerLookupResult = RequestRouter.FindRequestHandler("get", string.Empty, new Uri("http://test.com/level-1/2", UriKind.Absolute));
			}

			[Test]
			public void it_should_return_success()
			{
				_routeHandlerLookupResult.IsSuccessful.ShouldBe(true);
			}

			[Test]
			public void it_should_return_request_handler()
			{
				_routeHandlerLookupResult.RequestHandlerType.ShouldBe(typeof(DynamicLevel2RequestHandler));
			}

			[Test]
			public void it_should_bind_route_parameters()
			{
				_routeHandlerLookupResult.RouteParameters.Count.ShouldBe(1);
			}
		}

		[TestFixture]
		public class when_searching_for_request_handler_that_matches_mixed_multi_level_path : given_request_router_initialised_with_test_routes
		{
			private RouteHandlerLookupResult _routeHandlerLookupResult;

			[SetUp]
			public void SetUp()
			{
				_routeHandlerLookupResult = RequestRouter.FindRequestHandler("get", string.Empty, new Uri("http://test.com/level-1/2/level-3", UriKind.Absolute));
			}

			[Test]
			public void it_should_return_success()
			{
				_routeHandlerLookupResult.IsSuccessful.ShouldBe(true);
			}

			[Test]
			public void it_should_return_request_handler()
			{
				_routeHandlerLookupResult.RequestHandlerType.ShouldBe(typeof(MixedMultiLevelRequestHandler));
			}

			[Test]
			public void it_should_bind_route_parameters()
			{
				_routeHandlerLookupResult.RouteParameters.Count.ShouldBe(1);
			}
		}

		[TestFixture]
		public class when_searching_for_request_handler_that_matches_dynamic_multi_level_path : given_request_router_initialised_with_test_routes
		{
			private RouteHandlerLookupResult _routeHandlerLookupResult;

			[SetUp]
			public void SetUp()
			{
				_routeHandlerLookupResult = RequestRouter.FindRequestHandler("get", string.Empty, new Uri("http://test.com/1/2/3", UriKind.Absolute));
			}

			[Test]
			public void it_should_return_success()
			{
				_routeHandlerLookupResult.IsSuccessful.ShouldBe(true);
			}

			[Test]
			public void it_should_return_request_handler()
			{
				_routeHandlerLookupResult.RequestHandlerType.ShouldBe(typeof(DynamicMultiLevelRequestHandler));
			}

			[Test]
			public void it_should_bind_route_parameters()
			{
				_routeHandlerLookupResult.RouteParameters.Count.ShouldBe(3);
			}
		}

		[TestFixture]
		public class when_searching_for_request_handler_that_matches_static_path_adjacent_to_dynamic_path : given_request_router_initialised_with_test_routes
		{
			private RouteHandlerLookupResult _routeHandlerLookupResult;

			[SetUp]
			public void SetUp()
			{
				_routeHandlerLookupResult = RequestRouter.FindRequestHandler("get", string.Empty, new Uri("http://test.com/specificity/static", UriKind.Absolute));
			}

			[Test]
			public void it_should_return_success()
			{
				_routeHandlerLookupResult.IsSuccessful.ShouldBe(true);
			}

			[Test]
			public void it_should_return_request_handler()
			{
				_routeHandlerLookupResult.RequestHandlerType.ShouldBe(typeof(SpecificityTestStaticHandler));
			}
		}

		[TestFixture]
		public class when_searching_for_request_handler_that_matches_dynamic_multi_level_path_that_is_not_routed : given_request_router_initialised_with_test_routes
		{
			private RouteHandlerLookupResult _routeHandlerLookupResult;

			[SetUp]
			public void SetUp()
			{
				_routeHandlerLookupResult = RequestRouter.FindRequestHandler("get", string.Empty, new Uri("http://test.com/1/2/3/is-not-routed", UriKind.Absolute));
			}

			[Test]
			public void it_should_return_failed_result()
			{
				_routeHandlerLookupResult.ShouldBe(RouteHandlerLookupResult.FailedResult);
			}
		}

		[TestFixture]
		public class when_searching_for_request_handler : given_request_router_initialised_with_test_routes
		{
			private RouteHandlerLookupResult _routeHandlerLookupResult;

			[SetUp]
			public void SetUp()
			{
				_routeHandlerLookupResult = RequestRouter.FindRequestHandler(string.Empty, string.Empty, new Uri("http://test.com/", UriKind.Absolute));
			}

			[Test]
			public void it_should_return_failed_result()
			{
				_routeHandlerLookupResult.ShouldBe(RouteHandlerLookupResult.FailedResult);
			}
		}

		#endregion

		#region Invalid Configuration Detection

		[TestFixture]
		public class when_initialising_request_router_lookup_with_conflicting_routes
		{
			[Test]
			public void it_should_throw_exception()
			{
				var requestHandlers = new List<Type> {typeof(Handler1), typeof(Handler2)};
				Should.Throw<InvalidOperationException>(() => new RequestRouter(requestHandlers));
			}

			[Route("/route")]
			public class Handler1 : IGet<string>
			{
				[ExcludeFromCodeCoverage]
				public HttpResponseMessage<string> Get()
				{
					return null;
				}
			}

			[Route("/route")]
			public class Handler2 : IGet<string>
			{
				[ExcludeFromCodeCoverage]
				public HttpResponseMessage<string> Get()
				{
					return null;
				}
			}
		}

		[TestFixture]
		public class when_initialising_request_router_with_route_handler_that_does_not_implement_correct_interface
		{
			[Test]
			public void it_should_throw_exception()
			{
				var requestHandlers = new List<Type> {typeof(Handler)};
				Should.Throw<InvalidOperationException>(() => new RequestRouter(requestHandlers));
			}

			[Route("/")]
			public class Handler : IRequestHandler
			{
			}
		}

		[TestFixture]
		public class when_initialising_request_router_with_route_handler_that_does_not_expose_properties_declared_as_route_parameters
		{
			[Test]
			public void it_should_throw_exception()
			{
				var requestHandlers = new List<Type> {typeof(Handler)};
				Should.Throw<InvalidOperationException>(() => new RequestRouter(requestHandlers));
			}

			[Route("/{param}")]
			public class Handler : IGet<string>
			{
				[ExcludeFromCodeCoverage]
				public HttpResponseMessage<string> Get()
				{
					return null;
				}

				public int SomeOtherParam { get; set; }

				[Optional]
				public string OptionalParam { get; set; }
			}
		}

		[TestFixture]
		public class when_initialising_request_router_with_route_handler_that_does_have_a_route
		{
			[Test]
			public void it_should_throw_exception()
			{
				var requestHandlers = new List<Type> {typeof(Handler)};
				Should.Throw<InvalidOperationException>(() => new RequestRouter(requestHandlers));
			}

			public class Handler : IGet<string>
			{
				[ExcludeFromCodeCoverage]
				public HttpResponseMessage<string> Get()
				{
					return null;
				}
			}
		}

		#endregion

		public abstract class given_request_router_initialised_with_test_routes
		{
			protected IRequestRouter RequestRouter;

			private static readonly IEnumerable<Type> _testRoutes = new List<Type>
			{
				typeof(RootRequestHandler),
				typeof(StaticLevel1RequestHandler),
				typeof(StaticLevel2RequestHandler),
				typeof(DynamicLevel1RequestHandler),
				typeof(DynamicLevel2RequestHandler),
				typeof(MixedMultiLevelRequestHandler),
				typeof(DynamicMultiLevelRequestHandler),
				typeof(SpecificityTestDynamicHandler),
				typeof(SpecificityTestStaticHandler)
			};

			protected given_request_router_initialised_with_test_routes()
			{
				RequestRouter = new RequestRouter(_testRoutes);
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

			public Int32 DynamicLevel1 { get; set; }
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

		[Route("/level-1/{DynamicLevel2}/level-3")]
		public class MixedMultiLevelRequestHandler : IGet<string>
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

		[Route("/specificity/static")]
		public class SpecificityTestStaticHandler : IGet<string>
		{
			[ExcludeFromCodeCoverage]
			public HttpResponseMessage<string> Get()
			{
				return new HttpResponseMessage<string>(new HttpResponseMessage());
			}
		}

		[Route("/specificity/{dynamic}")]
		public class SpecificityTestDynamicHandler : IGet<string>
		{
			[ExcludeFromCodeCoverage]
			public HttpResponseMessage<string> Get()
			{
				return new HttpResponseMessage<string>(new HttpResponseMessage());
			}

			public string Dynamic { get; set; }
		}

		#endregion
	}
}