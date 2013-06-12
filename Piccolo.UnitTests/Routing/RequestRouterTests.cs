using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NUnit.Framework;
using Piccolo.Configuration;
using Piccolo.Routing;
using Shouldly;

namespace Piccolo.UnitTests.Routing
{
	public class RequestRouterTests
	{
		[TestFixture]
		public class when_searching_for_request_handler_for_uri_that_is_routed : given_request_router
		{
			private RequestRouterResult _requestRouterResult;

			[SetUp]
			public void SetUp()
			{
				_requestRouterResult = RequestRouter.FindRequestHandler("get", new Uri("https://api.com/data/resources/1"));
			}

			[Test]
			public void it_should_return_handler_type()
			{
				_requestRouterResult.HandlerType.ShouldBe(typeof(TestRequestHandler));
			}
		}

		[TestFixture]
		public class when_searching_for_request_handler_for_uri_that_is_not_routed : given_request_router
		{
			private RequestRouterResult _requestRouterResult;

			[SetUp]
			public void SetUp()
			{
				_requestRouterResult = RequestRouter.FindRequestHandler("get", new Uri("https://api.com/not-defined/1"));
			}

			[Test]
			public void it_should_return_null()
			{
				_requestRouterResult.HandlerType.ShouldBe(null);
			}
		}

		public abstract class given_request_router
		{
			protected IRequestRouter RequestRouter;

			protected given_request_router()
			{
				var httpHandlerConfiguration = new HttpHandlerConfiguration();
				httpHandlerConfiguration.RequestHandlers = new ReadOnlyCollection<Type>(new List<Type> {typeof(TestRequestHandler)});
				RequestRouter = new RequestRouter(httpHandlerConfiguration);
			}
		}

		#region Test Classes

		[Route("/data/resources/{id}")]
		public class TestRequestHandler : IRequestHandler
		{
			public int Id { get; set; }
		}

		#endregion
	}
}