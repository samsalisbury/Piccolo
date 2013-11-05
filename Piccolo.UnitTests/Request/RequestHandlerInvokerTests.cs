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

namespace Piccolo.UnitTests.Request
{
	public class RequestHandlerInvokerTests
	{
		#region Verbs

		[TestFixture]
		public class when_executing_get_request : given_request_handler_invoker
		{
			private string _result;

			[SetUp]
			public void SetUp()
			{
				_result = Invoker.Execute(new GetResource(), "GET", new Dictionary<string, string>(), new Dictionary<string, string>(), new Dictionary<string, object>(), string.Empty, null).Content.ReadAsStringAsync().Result;
			}

			[Test]
			public void it_should_bind_parameters()
			{
				_result.ShouldBe("TEST");
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
				_result = Invoker.Execute(new PostResource(), "POST", routeParameters, new Dictionary<string, string>(), new Dictionary<string, object>(), "\"\"", null).Content.ReadAsStringAsync().Result;
			}

			[Test]
			public void it_should_bind_parameters()
			{
				_result.ShouldBe("post");
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
				_result = Invoker.Execute(new PutResource(), "PUT", routeParameters, new Dictionary<string, string>(), new Dictionary<string, object>(), "\"\"", null).Content.ReadAsStringAsync().Result;
			}

			[Test]
			public void it_should_bind_parameters()
			{
				_result.ShouldBe("put");
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
				_result = Invoker.Execute(new DeleteResource(), "DELETE", routeParameters, new Dictionary<string, string>(), new Dictionary<string, object>(), "\"\"", null).Content.ReadAsStringAsync().Result;
			}

			[Test]
			public void it_should_bind_parameters()
			{
				_result.ShouldBe("delete");
			}
		}

		[Route("/RequestHandlerInvokerTests")]
		public class GetResource : IGet<string>
		{
			public HttpResponseMessage<string> Get()
			{
				return new HttpResponseMessage<string>(new HttpResponseMessage {Content = new StringContent("TEST")});
			}
		}

		[Route("/PostRequestHandlerInvokerTests/{Param}")]
		public class PostResource : IPost<string, string>
		{
			public HttpResponseMessage<string> Post(string parameters)
			{
				return new HttpResponseMessage<string>(new HttpResponseMessage {Content = new StringContent(Param)});
			}

			public string Param { get; set; }
		}

		[Route("/PutRequestHandlerInvokerTests/{Param}")]
		public class PutResource : IPut<string, string>
		{
			public HttpResponseMessage<string> Put(string parameters)
			{
				return new HttpResponseMessage<string>(new HttpResponseMessage {Content = new StringContent(Param)});
			}

			public string Param { get; set; }
		}

		[Route("/DeleteRequestHandlerInvokerTests/{Param}")]
		public class DeleteResource : IDelete<string, string>
		{
			public HttpResponseMessage<string> Delete(string parameters)
			{
				return new HttpResponseMessage<string>(new HttpResponseMessage {Content = new StringContent(Param)});
			}

			public string Param { get; set; }
		}

		#endregion

		#region Route Parameters

		[TestFixture]
		public class when_executing_get_request_with_string_parameter : given_request_handler_invoker
		{
			private string _result;

			[SetUp]
			public void SetUp()
			{
				var routeParameters = new Dictionary<string, string> {{"param", "TEST"}};
				_result = Invoker.Execute(new GetResourceString(), "GET", routeParameters, new Dictionary<string, string>(), new Dictionary<string, object>(), string.Empty, null).Content.ReadAsStringAsync().Result;
			}

			[Test]
			public void it_should_bind_parameters()
			{
				_result.ShouldBe("GET TEST");
			}
		}

		[TestFixture]
		public class when_executing_get_request_with_boolean_parameter : given_request_handler_invoker
		{
			private string _result;

			[SetUp]
			public void SetUp()
			{
				var routeParameters = new Dictionary<string, string> {{"param", "true"}};
				_result = Invoker.Execute(new GetResourceBoolean(), "GET", routeParameters, new Dictionary<string, string>(), new Dictionary<string, object>(), string.Empty, null).Content.ReadAsStringAsync().Result;
			}

			[Test]
			public void it_should_bind_parameters()
			{
				_result.ShouldBe("GET True");
			}
		}

		[TestFixture]
		public class when_executing_get_request_with_byte_parameter : given_request_handler_invoker
		{
			private string _result;

			[SetUp]
			public void SetUp()
			{
				var routeParameters = new Dictionary<string, string> {{"param", "1"}};
				_result = Invoker.Execute(new GetResourceByte(), "GET", routeParameters, new Dictionary<string, string>(), new Dictionary<string, object>(), string.Empty, null).Content.ReadAsStringAsync().Result;
			}

			[Test]
			public void it_should_bind_parameters()
			{
				_result.ShouldBe("GET 1");
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
				_result = Invoker.Execute(new GetResourceInt16(), "GET", routeParameters, new Dictionary<string, string>(), new Dictionary<string, object>(), string.Empty, null).Content.ReadAsStringAsync().Result;
			}

			[Test]
			public void it_should_bind_parameters()
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
				_result = Invoker.Execute(new GetResourceInt32(), "GET", routeParameters, new Dictionary<string, string>(), new Dictionary<string, object>(), string.Empty, null).Content.ReadAsStringAsync().Result;
			}

			[Test]
			public void it_should_bind_parameters()
			{
				_result.ShouldBe("GET 1");
			}
		}

		[TestFixture]
		public class when_executing_get_request_with_datetime_parameter : given_request_handler_invoker
		{
			private string _result;

			[SetUp]
			public void SetUp()
			{
				var routeParameters = new Dictionary<string, string> {{"param", "2013-07-22"}};
				_result = Invoker.Execute(new GetResourceDateTime(), "GET", routeParameters, new Dictionary<string, string>(), new Dictionary<string, object>(), string.Empty, null).Content.ReadAsStringAsync().Result;
			}

			[Test]
			public void it_should_bind_parameters()
			{
				_result.ShouldBe("GET 2013-07-22T00:00:00");
			}
		}

		[Route("/RequestHandlerInvokerTests/String/{Param}")]
		public class GetResourceString : IGet<string>
		{
			public HttpResponseMessage<string> Get()
			{
				return new HttpResponseMessage<string>(new HttpResponseMessage {Content = new StringContent(string.Format("GET {0}", Param))});
			}

			public String Param { get; set; }
		}

		[Route("/RequestHandlerInvokerTests/Boolean/{Param}")]
		public class GetResourceBoolean : IGet<string>
		{
			public HttpResponseMessage<string> Get()
			{
				return new HttpResponseMessage<string>(new HttpResponseMessage {Content = new StringContent(string.Format("GET {0}", Param))});
			}

			public Boolean Param { get; set; }
		}

		[Route("/RequestHandlerInvokerTests/Byte/{Param}")]
		public class GetResourceByte : IGet<string>
		{
			public HttpResponseMessage<string> Get()
			{
				return new HttpResponseMessage<string>(new HttpResponseMessage {Content = new StringContent(string.Format("GET {0}", Param))});
			}

			public Byte Param { get; set; }
		}

		[Route("/RequestHandlerInvokerTests/Int16/{Param}")]
		public class GetResourceInt16 : IGet<string>
		{
			public HttpResponseMessage<string> Get()
			{
				return new HttpResponseMessage<string>(new HttpResponseMessage {Content = new StringContent(string.Format("GET {0}", Param))});
			}

			public Int16 Param { get; set; }
		}

		[Route("/RequestHandlerInvokerTests/Int32/{Param}")]
		public class GetResourceInt32 : IGet<string>
		{
			public HttpResponseMessage<string> Get()
			{
				return new HttpResponseMessage<string>(new HttpResponseMessage {Content = new StringContent(string.Format("GET {0}", Param))});
			}

			public Int32 Param { get; set; }
		}

		[Route("/RequestHandlerInvokerTests/DateTime/{Param}")]
		public class GetResourceDateTime : IGet<string>
		{
			public HttpResponseMessage<string> Get()
			{
				return new HttpResponseMessage<string>(new HttpResponseMessage {Content = new StringContent(string.Format("GET {0:s}", Param))});
			}

			public DateTime Param { get; set; }
		}

		#endregion

		#region Query Parameters

		[TestFixture]
		public class when_executing_get_request_with_optional_string_parameter : given_request_handler_invoker
		{
			private string _result;

			[SetUp]
			public void SetUp()
			{
				var queryParameters = new Dictionary<string, string> {{"param", "TEST"}};
				_result = Invoker.Execute(new GetResourceOptionalString(), "GET", new Dictionary<string, string>(), queryParameters, new Dictionary<string, object>(), string.Empty, null).Content.ReadAsStringAsync().Result;
			}

			[Test]
			public void it_should_bind_parameters()
			{
				_result.ShouldBe("GET TEST");
			}
		}

		[TestFixture]
		public class when_executing_get_request_with_optional_boolean_parameter : given_request_handler_invoker
		{
			private string _result;

			[SetUp]
			public void SetUp()
			{
				var queryParameters = new Dictionary<string, string> {{"param", "true"}};
				_result = Invoker.Execute(new GetResourceOptionalBoolean(), "GET", new Dictionary<string, string>(), queryParameters, new Dictionary<string, object>(), string.Empty, null).Content.ReadAsStringAsync().Result;
			}

			[Test]
			public void it_should_bind_parameters()
			{
				_result.ShouldBe("GET True");
			}
		}

		[TestFixture]
		public class when_executing_get_request_with_optional_nullable_boolean_parameter : given_request_handler_invoker
		{
			private string _result;

			[SetUp]
			public void SetUp()
			{
				var queryParameters = new Dictionary<string, string> {{"param", "true"}};
				_result = Invoker.Execute(new GetResourceOptionalNullableBoolean(), "GET", new Dictionary<string, string>(), queryParameters, new Dictionary<string, object>(), string.Empty, null).Content.ReadAsStringAsync().Result;
			}

			[Test]
			public void it_should_bind_parameters()
			{
				_result.ShouldBe("GET True");
			}
		}

		[TestFixture]
		public class when_executing_get_request_with_optional_byte_parameter : given_request_handler_invoker
		{
			private string _result;

			[SetUp]
			public void SetUp()
			{
				var queryParameters = new Dictionary<string, string> {{"param", "1"}};
				_result = Invoker.Execute(new GetResourceOptionalByte(), "GET", new Dictionary<string, string>(), queryParameters, new Dictionary<string, object>(), string.Empty, null).Content.ReadAsStringAsync().Result;
			}

			[Test]
			public void it_should_bind_parameters()
			{
				_result.ShouldBe("GET 1");
			}
		}

		[TestFixture]
		public class when_executing_get_request_with_optional_nullable_byte_parameter : given_request_handler_invoker
		{
			private string _result;

			[SetUp]
			public void SetUp()
			{
				var queryParameters = new Dictionary<string, string> {{"param", "1"}};
				_result = Invoker.Execute(new GetResourceOptionalNullableByte(), "GET", new Dictionary<string, string>(), queryParameters, new Dictionary<string, object>(), string.Empty, null).Content.ReadAsStringAsync().Result;
			}

			[Test]
			public void it_should_bind_parameters()
			{
				_result.ShouldBe("GET 1");
			}
		}

		[TestFixture]
		public class when_executing_get_request_with_optional_int16_parameter : given_request_handler_invoker
		{
			private string _result;

			[SetUp]
			public void SetUp()
			{
				var queryParameters = new Dictionary<string, string> {{"param", "1"}};
				_result = Invoker.Execute(new GetResourceOptionalInt16(), "GET", new Dictionary<string, string>(), queryParameters, new Dictionary<string, object>(), string.Empty, null).Content.ReadAsStringAsync().Result;
			}

			[Test]
			public void it_should_bind_parameters()
			{
				_result.ShouldBe("GET 1");
			}
		}

		[TestFixture]
		public class when_executing_get_request_with_optional_nullable_int16_parameter : given_request_handler_invoker
		{
			private string _result;

			[SetUp]
			public void SetUp()
			{
				var queryParameters = new Dictionary<string, string> {{"param", "1"}};
				_result = Invoker.Execute(new GetResourceOptionalNullableInt16(), "GET", new Dictionary<string, string>(), queryParameters, new Dictionary<string, object>(), string.Empty, null).Content.ReadAsStringAsync().Result;
			}

			[Test]
			public void it_should_bind_parameters()
			{
				_result.ShouldBe("GET 1");
			}
		}

		[TestFixture]
		public class when_executing_get_request_with_optional_int32_parameter : given_request_handler_invoker
		{
			private string _result;

			[SetUp]
			public void SetUp()
			{
				var queryParameters = new Dictionary<string, string> {{"param", "1"}};
				_result = Invoker.Execute(new GetResourceOptionalInt32(), "GET", new Dictionary<string, string>(), queryParameters, new Dictionary<string, object>(), string.Empty, null).Content.ReadAsStringAsync().Result;
			}

			[Test]
			public void it_should_bind_parameters()
			{
				_result.ShouldBe("GET 1");
			}
		}

		[TestFixture]
		public class when_executing_get_request_with_optional_nullable_int32_parameter : given_request_handler_invoker
		{
			private string _result;

			[SetUp]
			public void SetUp()
			{
				var queryParameters = new Dictionary<string, string> {{"param", "1"}};
				_result = Invoker.Execute(new GetResourceOptionalNullableInt32(), "GET", new Dictionary<string, string>(), queryParameters, new Dictionary<string, object>(), string.Empty, null).Content.ReadAsStringAsync().Result;
			}

			[Test]
			public void it_should_bind_parameters()
			{
				_result.ShouldBe("GET 1");
			}
		}

		[TestFixture]
		public class when_executing_get_request_with_optional_datetime_parameter : given_request_handler_invoker
		{
			private string _result;

			[SetUp]
			public void SetUp()
			{
				var queryParameters = new Dictionary<string, string> {{"param", "2013-07-22"}};
				_result = Invoker.Execute(new GetResourceOptionalDateTime(), "GET", new Dictionary<string, string>(), queryParameters, new Dictionary<string, object>(), string.Empty, null).Content.ReadAsStringAsync().Result;
			}

			[Test]
			public void it_should_bind_parameters()
			{
				_result.ShouldBe("GET 2013-07-22T00:00:00");
			}
		}

		[TestFixture]
		public class when_executing_get_request_with_optional_nullable_datetime_parameter : given_request_handler_invoker
		{
			private string _result;

			[SetUp]
			public void SetUp()
			{
				var queryParameters = new Dictionary<string, string> {{"param", "2013-07-22"}};
				_result = Invoker.Execute(new GetResourceOptionalNullableDateTime(), "GET", new Dictionary<string, string>(), queryParameters, new Dictionary<string, object>(), string.Empty, null).Content.ReadAsStringAsync().Result;
			}

			[Test]
			public void it_should_bind_parameters()
			{
				_result.ShouldBe("GET 2013-07-22T00:00:00");
			}
		}

		[TestFixture]
		public class when_executing_get_request_with_redundant_query_parameter : given_request_handler_invoker
		{
			private string _result;

			[SetUp]
			public void SetUp()
			{
				var queryParameters = new Dictionary<string, string> {{"redundant", ""}};
				_result = Invoker.Execute(new GetResourceOptionalString(), "GET", new Dictionary<string, string>(), queryParameters, new Dictionary<string, object>(), string.Empty, null).Content.ReadAsStringAsync().Result;
			}

			[Test]
			public void it_should_bind_parameters()
			{
				_result.ShouldBe("GET ");
			}
		}

		[Route("/RequestHandlerInvokerTests/String/Optional")]
		public class GetResourceOptionalString : IGet<string>
		{
			public HttpResponseMessage<string> Get()
			{
				return new HttpResponseMessage<string>(new HttpResponseMessage {Content = new StringContent(string.Format("GET {0}", Param))});
			}

			[Optional]
			public String Param { get; set; }
		}

		[Route("/RequestHandlerInvokerTests/Boolean/Optional")]
		public class GetResourceOptionalBoolean : IGet<string>
		{
			public HttpResponseMessage<string> Get()
			{
				return new HttpResponseMessage<string>(new HttpResponseMessage {Content = new StringContent(string.Format("GET {0}", Param))});
			}

			[Optional]
			public Boolean Param { get; set; }
		}

		[Route("/RequestHandlerInvokerTests/Boolean/OptionalNullable")]
		public class GetResourceOptionalNullableBoolean : IGet<string>
		{
			public HttpResponseMessage<string> Get()
			{
				return new HttpResponseMessage<string>(new HttpResponseMessage {Content = new StringContent(string.Format("GET {0}", Param))});
			}

			[Optional]
			public Boolean? Param { get; set; }
		}

		[Route("/RequestHandlerInvokerTests/Byte/Optional")]
		public class GetResourceOptionalByte : IGet<string>
		{
			public HttpResponseMessage<string> Get()
			{
				return new HttpResponseMessage<string>(new HttpResponseMessage {Content = new StringContent(string.Format("GET {0}", Param))});
			}

			[Optional]
			public Byte Param { get; set; }
		}

		[Route("/RequestHandlerInvokerTests/Byte/OptionalNullable")]
		public class GetResourceOptionalNullableByte : IGet<string>
		{
			public HttpResponseMessage<string> Get()
			{
				return new HttpResponseMessage<string>(new HttpResponseMessage {Content = new StringContent(string.Format("GET {0}", Param))});
			}

			[Optional]
			public Byte? Param { get; set; }
		}

		[Route("/RequestHandlerInvokerTests/Int16/Optional")]
		public class GetResourceOptionalInt16 : IGet<string>
		{
			public HttpResponseMessage<string> Get()
			{
				return new HttpResponseMessage<string>(new HttpResponseMessage {Content = new StringContent(string.Format("GET {0}", Param))});
			}

			[Optional]
			public Int16 Param { get; set; }
		}

		[Route("/RequestHandlerInvokerTests/Int16/OptionalNullable")]
		public class GetResourceOptionalNullableInt16 : IGet<string>
		{
			public HttpResponseMessage<string> Get()
			{
				return new HttpResponseMessage<string>(new HttpResponseMessage {Content = new StringContent(string.Format("GET {0}", Param))});
			}

			[Optional]
			public Int16? Param { get; set; }
		}

		[Route("/RequestHandlerInvokerTests/Int32/Optional")]
		public class GetResourceOptionalInt32 : IGet<string>
		{
			public HttpResponseMessage<string> Get()
			{
				return new HttpResponseMessage<string>(new HttpResponseMessage {Content = new StringContent(string.Format("GET {0}", Param))});
			}

			[Optional]
			public Int32 Param { get; set; }
		}

		[Route("/RequestHandlerInvokerTests/Int32/OptionalNullable")]
		public class GetResourceOptionalNullableInt32 : IGet<string>
		{
			public HttpResponseMessage<string> Get()
			{
				return new HttpResponseMessage<string>(new HttpResponseMessage {Content = new StringContent(string.Format("GET {0}", Param))});
			}

			[Optional]
			public Int32? Param { get; set; }
		}

		[Route("/RequestHandlerInvokerTests/DateTime/Optional")]
		public class GetResourceOptionalDateTime : IGet<string>
		{
			public HttpResponseMessage<string> Get()
			{
				return new HttpResponseMessage<string>(new HttpResponseMessage {Content = new StringContent(string.Format("GET {0:s}", Param))});
			}

			[Optional]
			public DateTime Param { get; set; }
		}

		[Route("/RequestHandlerInvokerTests/DateTime/OptionalNullable")]
		public class GetResourceOptionalNullableDateTime : IGet<string>
		{
			public HttpResponseMessage<string> Get()
			{
				return new HttpResponseMessage<string>(new HttpResponseMessage {Content = new StringContent(string.Format("GET {0:s}", Param))});
			}

			[Optional]
			public DateTime? Param { get; set; }
		}

		#endregion

		#region Contextual Parameters

		[TestFixture]
		public class when_executing_get_request_with_contextual_parameter : given_request_handler_invoker
		{
			private string _result;

			[SetUp]
			public void SetUp()
			{
				var contextualParameters = new Dictionary<string, object> {{"Param", new MyClass {Value = "TEST"}}};
				_result = Invoker.Execute(new GetResourceContextual(), "GET", new Dictionary<string, string>(), new Dictionary<string, string>(), contextualParameters, string.Empty, null).Content.ReadAsStringAsync().Result;
			}

			[Test]
			public void it_should_bind_parameters()
			{
				_result.ShouldBe("GET TEST");
			}
		}

		[Route("/RequestHandlerInvokerTests/Contextual")]
		public class GetResourceContextual : IGet<string>
		{
			public HttpResponseMessage<string> Get()
			{
				return new HttpResponseMessage<string>(new HttpResponseMessage {Content = new StringContent(string.Format("GET {0}", Param.Value))});
			}

			[Contextual]
			public MyClass Param { get; set; }
		}

		public class MyClass
		{
			public string Value { get; set; }
		}

		#endregion

		#region PUT/POST Parameters

		[TestFixture]
		public class when_executing_post_request_with_payload : given_request_handler_invoker
		{
			private string _result;

			[SetUp]
			public void SetUp()
			{
				var payload = "{" +
				              "\"A\":1," +
				              "\"B\":\"2\"" +
				              "}";
				_result = Invoker.Execute(new PostResourceWithPayload(), "POST", new Dictionary<string, string>(), new Dictionary<string, string>(), new Dictionary<string, object>(), payload, null).Content.ReadAsStringAsync().Result;
			}

			[Test]
			public void it_should_bind_parameters()
			{
				_result.ShouldBe("A: 1; B: 2");
			}
		}

		[Route("/PutRequestHandlerInvokerTests/Payload")]
		public class PostResourceWithPayload : IPost<PostResourceWithPayload.Parameters, PostResourceWithPayload.Parameters>
		{
			public HttpResponseMessage<Parameters> Post(Parameters parameters)
			{
				var content = string.Format("A: {0}; B: {1}", parameters.A, parameters.B);
				return new HttpResponseMessage<Parameters>(new HttpResponseMessage {Content = new StringContent(content)});
			}

			public class Parameters
			{
				public int A { get; set; }
				public string B { get; set; }
			}
		}

		[TestFixture]
		public class when_executing_put_request_with_payload : given_request_handler_invoker
		{
			private string _result;

			[SetUp]
			public void SetUp()
			{
				var payload = "{" +
				              "\"A\":1," +
				              "\"B\":\"2\"" +
				              "}";
				_result = Invoker.Execute(new PutResourceWithPayload(), "PUT", new Dictionary<string, string>(), new Dictionary<string, string>(), new Dictionary<string, object>(), payload, null).Content.ReadAsStringAsync().Result;
			}

			[Test]
			public void it_should_bind_parameters()
			{
				_result.ShouldBe("A: 1; B: 2");
			}
		}

		[Route("/PutRequestHandlerInvokerTests/Payload")]
		public class PutResourceWithPayload : IPut<PutResourceWithPayload.Parameters, PutResourceWithPayload.Parameters>
		{
			public HttpResponseMessage<Parameters> Put(Parameters parameters)
			{
				var content = string.Format("A: {0}; B: {1}", parameters.A, parameters.B);
				return new HttpResponseMessage<Parameters>(new HttpResponseMessage {Content = new StringContent(content)});
			}

			public class Parameters
			{
				public int A { get; set; }
				public string B { get; set; }
			}
		}

		[TestFixture]
		public class when_executing_delete_request_with_payload : given_request_handler_invoker
		{
			private string _result;

			[SetUp]
			public void SetUp()
			{
				var payload = "{" +
				              "\"A\":1," +
				              "\"B\":\"2\"" +
				              "}";
				_result = Invoker.Execute(new DeleteResourceWithPayload(), "DELETE", new Dictionary<string, string>(), new Dictionary<string, string>(), new Dictionary<string, object>(), payload, null).Content.ReadAsStringAsync().Result;
			}

			[Test]
			public void it_should_bind_parameters()
			{
				_result.ShouldBe("A: 1; B: 2");
			}
		}

		[Route("/PutRequestHandlerInvokerTests/Payload")]
		public class DeleteResourceWithPayload : IDelete<DeleteResourceWithPayload.Parameters, DeleteResourceWithPayload.Parameters>
		{
			public HttpResponseMessage<Parameters> Delete(Parameters parameters)
			{
				var content = string.Format("A: {0}; B: {1}", parameters.A, parameters.B);
				return new HttpResponseMessage<Parameters>(new HttpResponseMessage {Content = new StringContent(content)});
			}

			public class Parameters
			{
				public int A { get; set; }
				public string B { get; set; }
			}
		}

		#endregion

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
				Invoker = new RequestHandlerInvoker(configuration.JsonDeserialiser, configuration.ParameterBinders);
			}
		}
	}
}