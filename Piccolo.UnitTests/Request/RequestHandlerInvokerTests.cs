using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Reflection;
using NUnit.Framework;
using Piccolo.Configuration;
using Piccolo.Request;
using Piccolo.Validation;
using Shouldly;

namespace Piccolo.Tests.Request
{
	public class RequestHandlerInvokerTests
	{
		#region Validation

		[TestFixture]
		public class when_executing_request_with_validator_with_valid_payload : given_request_handler_invoker
		{
			private HttpResponseMessage _result;

			[SetUp]
			public void SetUp()
			{
				const string payload = "{" +
				                       "\"A\":\"2\"" +
				                       "}";
				_result = Invoker.Execute(new TestResourceWithPayload(), "POST", new Dictionary<string, string>(), new Dictionary<string, string>(), new Dictionary<string, object>(), payload, new TestPayloadValidator());
			}

			[Test]
			public void it_should_pass_validation()
			{
				_result.StatusCode.ShouldBe(HttpStatusCode.OK);
			}
		}

		[TestFixture]
		public class when_executing_request_with_validator_with_invalid_payload : given_request_handler_invoker
		{
			private HttpResponseMessage _result;

			[SetUp]
			public void SetUp()
			{
				const string payload = "{ \"A\" : \"\" }";
				_result = Invoker.Execute(new TestResourceWithPayload(), "POST", new Dictionary<string, string>(), new Dictionary<string, string>(), new Dictionary<string, object>(), payload, new TestPayloadValidator());
			}

			[Test]
			public void it_should_fail_validation()
			{
				_result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
			}

			[Test]
			public void it_should_return_error_message()
			{
				var content = ((ObjectContent)_result.Content).Content;
				content.GetType().GetProperty("error").GetValue(content, null).ShouldBe("invalid");
			}
		}

		[TestFixture]
		public class when_executing_request_with_validator_with_valid_optional_parameter : given_request_handler_invoker
		{
			private HttpResponseMessage _result;

			[SetUp]
			public void SetUp()
			{
				var queryParameters = new Dictionary<string, string> {{"param", "1"}};
				_result = Invoker.Execute(new TestResourceWithOptioanalParameter(), "GET", new Dictionary<string, string>(), queryParameters, new Dictionary<string, object>(), null, null);
			}

			[Test]
			public void it_should_pass_validation()
			{
				_result.StatusCode.ShouldBe(HttpStatusCode.OK);
			}
		}

		[TestFixture]
		public class when_executing_request_with_validator_with_invalid_optional_parameter : given_request_handler_invoker
		{
			private HttpResponseMessage _result;

			[SetUp]
			public void SetUp()
			{
				var queryParameters = new Dictionary<string, string> {{"param", "-1"}};
				_result = Invoker.Execute(new TestResourceWithOptioanalParameter(), "GET", new Dictionary<string, string>(), queryParameters, new Dictionary<string, object>(), null, null);
			}

			[Test]
			public void it_should_fail_validation()
			{
				_result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
			}

			[Test]
			public void it_should_return_error_message()
			{
				var content = ((ObjectContent)_result.Content).Content;
				content.GetType().GetProperty("error").GetValue(content, null).ShouldBe("invalid age");
			}
		}

		[Route("/RequestHandlerInvokerTests/Validation/Payload")]
		public class TestResourceWithPayload : IPost<TestResourceWithPayload.Parameters, string>
		{
			public HttpResponseMessage<string> Post(Parameters parameters)
			{
				return Response.Success.Ok(String.Empty);
			}

			public class Parameters
			{
				public string A { get; set; }
			}
		}

		[Route("/RequestHandlerInvokerTests/Validation/OptionalParameter")]
		public class TestResourceWithOptioanalParameter : IGet<string>
		{
			[Optional]
			[ValidateWith(typeof(TestParameterValidator))]
			public int Param { get; set; }

			public HttpResponseMessage<string> Get()
			{
				return Response.Success.Ok(String.Empty);
			}
		}

		#endregion

		public abstract class given_request_handler_invoker
		{
			protected RequestHandlerInvoker Invoker;

			protected given_request_handler_invoker()
			{
				var configuration = Bootstrapper.ApplyConfiguration(Assembly.GetExecutingAssembly(), false);
				Invoker = new RequestHandlerInvoker(configuration.JsonDeserialiser, configuration.Parsers);
			}
		}
	}
}