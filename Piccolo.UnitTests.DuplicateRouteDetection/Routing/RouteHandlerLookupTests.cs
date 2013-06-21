﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using Piccolo.Routing;
using Shouldly;

namespace Piccolo.UnitTests.DuplicateRouteDetection.Routing
{
	public class RouteHandlerLookupTests
	{
		[TestFixture]
		public class when_initialising_route_handler_lookup_with_conflicting_routes
		{
			[Test]
			public void it_should_throw_exception()
			{
				var requestHandlers = new List<Type> {typeof(Handler1), typeof(Handler2)};
				Should.Throw<InvalidOperationException>(() => new RouteHandlerLookup(requestHandlers));
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
	}
}