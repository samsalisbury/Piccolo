using System;
using System.Linq;
using System.Reflection;
using Piccolo.IoC;

namespace Piccolo.Configuration
{
	public class Bootstrapper
	{
		private readonly Assembly _assembly;

		public Bootstrapper(Assembly assembly)
		{
			_assembly = assembly;
		}

		public HttpHandlerConfiguration ApplyConfiguration(bool applyCustomConfiguration)
		{
			var configuration = new HttpHandlerConfiguration();

			ApplyDefaultConfiguration(configuration);
			if (applyCustomConfiguration)
				ApplyCustomConfiguration(configuration, _assembly);

			return configuration;
		}

		private static void ApplyDefaultConfiguration(HttpHandlerConfiguration configuration)
		{
			configuration.RequestHandlerFactory = new DefaultRequestHandlerFactory();
		}

		private static void ApplyCustomConfiguration(HttpHandlerConfiguration configuration, Assembly assembly)
		{
			var exportedTypes = assembly.GetExportedTypes();
			var bootstrapperTypes = exportedTypes.Where(x => x.GetInterfaces().Contains(typeof(IStartupTask)));

			foreach (var bootstrapperType in bootstrapperTypes)
			{
				var instance = (IStartupTask)Activator.CreateInstance(bootstrapperType);
				instance.Run(configuration);
			}
		}
	}
}