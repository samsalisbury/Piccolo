using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using NUnit.Framework;
using Piccolo.Configuration;
using Piccolo.Validation;
using Shouldly;

namespace Piccolo.Tests.Validation
{
	public class PayloadValidatorInvokerTests
	{
		[TestFixture]
		public class when_validating_valid_payload : given_payload_validator_invoker
		{
			private string _errorMessage;

			[SetUp]
			public void SetUp()
			{
				_errorMessage = PayloadValidatorInvoker.ValidatePayload(typeof(PostResource), "POST", "\"payload\"");
			}

			[Test]
			public void it_should_return_null()
			{
				_errorMessage.ShouldBe(null);
			}

			[ExcludeFromCodeCoverage]
			[ValidateWith(typeof(Validator))]
			public class PostResource : IPost<string, string>
			{
				public HttpResponseMessage<string> Post(string parameters)
				{
					return null;
				}
			}

			public class Validator : IPayloadValidator<string>
			{
				public ValidationResult Validate(string payload)
				{
					return ValidationResult.Valid;
				}
			}
		}

		[TestFixture]
		public class when_validating_invalid_payload : given_payload_validator_invoker
		{
			private string _errorMessage;

			[SetUp]
			public void SetUp()
			{
				_errorMessage = PayloadValidatorInvoker.ValidatePayload(typeof(PostResource), "POST", "\"payload\"");
			}

			[Test]
			public void it_should_return_error_message()
			{
				_errorMessage.ShouldBe("meh");
			}

			[ExcludeFromCodeCoverage]
			[ValidateWith(typeof(Validator))]
			public class PostResource : IPost<string, string>
			{
				public HttpResponseMessage<string> Post(string parameters)
				{
					return null;
				}
			}

			public class Validator : IPayloadValidator<string>
			{
				public ValidationResult Validate(string payload)
				{
					return new ValidationResult("meh");
				}
			}
		}

		[TestFixture]
		public class when_validating_payload_with_validator_that_is_not_actually_a_validator : given_payload_validator_invoker
		{
			[Test]
			public void it_should_throw_exception()
			{
				Assert.Throws<InvalidPayloadValidatorException>(() => PayloadValidatorInvoker.ValidatePayload(typeof(PostResource), "POST", "\"payload\""));
			}

			[ExcludeFromCodeCoverage]
			[ValidateWith(typeof(string))]
			public class PostResource : IPost<string, string>
			{
				public HttpResponseMessage<string> Post(string parameters)
				{
					return null;
				}
			}
		}

		[TestFixture]
		public class when_validating_payload_with_validator_that_targets_a_different_type_for_validation : given_payload_validator_invoker
		{
			[Test]
			public void it_should_throw_exception()
			{
				Assert.Throws<InvalidPayloadValidatorException>(() => PayloadValidatorInvoker.ValidatePayload(typeof(PostResource), "POST", "\"payload\""));
			}

			[ExcludeFromCodeCoverage]
			[ValidateWith(typeof(Validator))]
			public class PostResource : IPost<string, string>
			{
				public HttpResponseMessage<string> Post(string parameters)
				{
					return null;
				}
			}

			[ExcludeFromCodeCoverage]
			public class Validator : IPayloadValidator<int>
			{
				public ValidationResult Validate(int payload)
				{
					return ValidationResult.Valid;
				}
			}
		}

		public abstract class given_payload_validator_invoker
		{
			protected IPayloadValidatorInvoker PayloadValidatorInvoker;

			protected given_payload_validator_invoker()
			{
				var configuration = Bootstrapper.ApplyConfiguration(Assembly.GetCallingAssembly(), false);
				PayloadValidatorInvoker = new PayloadValidatorInvoker(configuration.ObjectFactory);
			}
		}
	}
}