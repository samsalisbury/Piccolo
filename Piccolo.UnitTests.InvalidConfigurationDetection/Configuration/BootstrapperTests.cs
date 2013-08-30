using System;
using System.Web;
using NUnit.Framework;
using Piccolo.Configuration;
using Shouldly;

namespace Piccolo.UnitTests.InvalidConfigurationDetection.Configuration
{
	public class BootstrapperTests
	{
		[TestFixture]
		public class when_building_configuration_with_invalid_assembly
		{
			[Test]
			public void it_should_throw_exception()
			{
				Should.Throw<InvalidOperationException>(() => Bootstrapper.ApplyConfiguration(typeof(HttpApplication).BaseType.Assembly, false));
			}
		}
	}
}