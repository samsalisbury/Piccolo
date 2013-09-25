using System.Net;
using NUnit.Framework;
using Shouldly;

namespace Piccolo.UnitTests
{
	public class ResponseTests
	{
		public class Error
		{
			[TestFixture]
			public class when_bad_request_response_message_is_returned
			{
				private HttpResponseMessage<object> _response;

				[SetUp]
				public void SetUp()
				{
					_response = Response.Error.BadRequest<object>("bad request");
				}

				[Test]
				public void status_code_should_be_400()
				{
					_response.Message.StatusCode.ShouldBe((HttpStatusCode)400);
				}

				[Test]
				public void reason_phrase_should_be_not_found()
				{
					_response.Message.ReasonPhrase.ShouldBe("Bad Request");
				}
			}

			[TestFixture]
			public class when_not_found_response_message_is_returned
			{
				private HttpResponseMessage<object> _response;

				[SetUp]
				public void SetUp()
				{
					_response = Response.Error.NotFound<object>();
				}

				[Test]
				public void status_code_should_be_404()
				{
					_response.Message.StatusCode.ShouldBe((HttpStatusCode)404);
				}

				[Test]
				public void reason_phrase_should_be_not_found()
				{
					_response.Message.ReasonPhrase.ShouldBe("Not Found");
				}
			}

			[TestFixture]
			public class when_gone_response_message_is_returned
			{
				private HttpResponseMessage<object> _response;

				[SetUp]
				public void SetUp()
				{
					_response = Response.Error.Gone<object>("gone");
				}

				[Test]
				public void status_code_should_be_410()
				{
					_response.Message.StatusCode.ShouldBe((HttpStatusCode)410);
				}

				[Test]
				public void reason_phrase_should_be_gone()
				{
					_response.Message.ReasonPhrase.ShouldBe("Gone");
				}
			}
		}

		public class Success
		{
			[TestFixture]
			public class when_ok_response_message_is_returned
			{
				private HttpResponseMessage<object> _response;

				[SetUp]
				public void SetUp()
				{
					_response = Response.Success.Ok<object>(null);
				}

				[Test]
				public void status_code_should_be_200()
				{
					_response.Message.StatusCode.ShouldBe((HttpStatusCode)200);
				}

				[Test]
				public void reason_phrase_should_be_ok()
				{
					_response.Message.ReasonPhrase.ShouldBe("OK");
				}
			}

			[TestFixture]
			public class when_created_response_message_is_returned
			{
				private HttpResponseMessage<object> _response;

				[SetUp]
				public void SetUp()
				{
					_response = Response.Success.Created<object>(null);
				}

				[Test]
				public void status_code_should_be_201()
				{
					_response.Message.StatusCode.ShouldBe((HttpStatusCode)201);
				}

				[Test]
				public void reason_phrase_should_be_created()
				{
					_response.Message.ReasonPhrase.ShouldBe("Created");
				}
			}

			[TestFixture]
			public class when_no_content_response_message_is_returned
			{
				private HttpResponseMessage<object> _response;

				[SetUp]
				public void SetUp()
				{
					_response = Response.Success.NoContent<object>();
				}

				[Test]
				public void status_code_should_be_204()
				{
					_response.Message.StatusCode.ShouldBe((HttpStatusCode)204);
				}

				[Test]
				public void reason_phrase_should_be_no_content()
				{
					_response.Message.ReasonPhrase.ShouldBe("No Content");
				}
			}
		}

		[TestFixture]
		public class when_error_response_is_created
		{
			private HttpResponseMessage<object> _response;

			[SetUp]
			public void SetUp()
			{
				_response = Response.CreateErrorResponse<object>(HttpStatusCode.Forbidden, "test");
			}

			[Test]
			public void status_code_should_be_403()
			{
				_response.Message.StatusCode.ShouldBe((HttpStatusCode)403);
			}

			[Test]
			public void reason_phrase_should_be_forbidden()
			{
				_response.Message.ReasonPhrase.ShouldBe("Forbidden");
			}

			[Test]
			public void content_should_be_test()
			{
				((ObjectContent)_response.Message.Content).Content.ShouldBe("test");
			}
		}
	}
}