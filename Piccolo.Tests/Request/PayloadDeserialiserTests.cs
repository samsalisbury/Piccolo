using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using NUnit.Framework;
using Piccolo.Configuration;
using Piccolo.Request;
using Piccolo.Tests.Validation;
using Shouldly;

namespace Piccolo.Tests.Request
{
	public class PayloadDeserialiserTests
	{
		[TestFixture]
		public class when_deserialising_payload : given_payload_deserialiser
		{
			private object _payload;

			[SetUp]
			public void SetUp()
			{
				const string payload = "\"test\"";

				_payload = Deserialiser.DeserialisePayload(new PostResource(), "POST", payload);
			}

			[Test]
			public void it_should_deserialise()
			{
				_payload.ShouldBe("test");
			}

			[ExcludeFromCodeCoverage]
			public class PostResource : IPost<string, string>
			{
				public HttpResponseMessage<string> Post(string parameters)
				{
					return null;
				}
			}
		}

		[TestFixture]
		public class when_deserialising_missing_payload : given_payload_deserialiser
		{
			[Test]
			public void it_should_throw_exception()
			{
				const string payload = null;

				Assert.Throws<MissingPayloadException>(() => Deserialiser.DeserialisePayload(new PostResource(), "POST", payload));
			}

			[ExcludeFromCodeCoverage]
			public class PostResource : IPost<string, string>
			{
				public HttpResponseMessage<string> Post(string parameters)
				{
					return null;
				}
			}
		}

		[TestFixture]
		public class when_deserialising_malformed_payload : given_payload_deserialiser
		{
			[Test]
			public void it_should_throw_exception()
			{
				const string payload = "{";

				Assert.Throws<MalformedPayloadException>(() => Deserialiser.DeserialisePayload(new PostResource(), "POST", payload));
			}

			[ExcludeFromCodeCoverage]
			public class PostResource : IPost<string, string>
			{
				public HttpResponseMessage<string> Post(string parameters)
				{
					return null;
				}
			}
		}

		[TestFixture]
		public class when_deserialising_malformed_payload_parameter : given_payload_deserialiser
		{
			[Test]
			public void it_should_throw_exception()
			{
				const string payload = "{ dateTime: \"\" }";

				Assert.Throws<MalformedPayloadException>(() => Deserialiser.DeserialisePayload(new PostResource(), "POST", payload));
			}

			[ExcludeFromCodeCoverage]
			public class PostResource : IPost<Params, string>
			{
				public HttpResponseMessage<string> Post(Params parameters)
				{
					return null;
				}
			}

			public class Params
			{
				public DateTime DateTime { get; set; }
			}
		}

		[TestFixture]
		public class when_deserialiser_throws_format_exception : given_payload_deserialiser
		{
			[Test]
			public void it_should_throw_malformed_payload_exception()
			{
				var deserialiser = new PayloadDeserialiser((_, __) => { throw new FormatException(); });

				Assert.Throws<MalformedPayloadException>(() => deserialiser.DeserialisePayload(new PostResource(), "POST", "{}"));
			}

			[ExcludeFromCodeCoverage]
			private class PostResource : IPost<object, string>
			{
				public HttpResponseMessage<string> Post(object parameters)
				{
					return null;
				}
			}
		}

		public abstract class given_payload_deserialiser
		{
			protected IPayloadDeserialiser Deserialiser;

			protected given_payload_deserialiser()
			{
				var configuration = Bootstrapper.ApplyConfiguration(Assembly.GetCallingAssembly(), false);
				Deserialiser = new PayloadDeserialiser(configuration.JsonDeserialiser);
			}
		}
	}
}