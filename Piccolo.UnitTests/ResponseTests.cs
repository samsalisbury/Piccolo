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
		}
	}
}