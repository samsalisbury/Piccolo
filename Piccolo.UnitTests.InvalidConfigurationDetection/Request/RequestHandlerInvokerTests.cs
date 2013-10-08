using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using Piccolo.Request;
using Piccolo.Request.ParameterBinders;
using Shouldly;

namespace Piccolo.UnitTests.InvalidConfigurationDetection.Request
{
	[TestFixture]
	public class when_binding_query_parameter_of_unsupported_type
	{
		[Test]
		public void it_should_throw_exception()
		{
			var handlerInvoker = new RequestHandlerInvoker(null, new Dictionary<Type, IParameterBinder>());

			Should.Throw<InvalidOperationException>(() =>
			{
				var queryParameters = new Dictionary<string, string> {{"Param", "value"}};
				handlerInvoker.Execute(new Handler(), "GET", new Dictionary<string, string>(), queryParameters, new Dictionary<string, object>(), null, null);
			});
		}

		[Route("/route")]
		public class Handler : IGet<string>
		{
			[Optional]
			public uint Param { get; set; }

			[ExcludeFromCodeCoverage]
			public HttpResponseMessage<string> Get()
			{
				return null;
			}
		}
	}
}