using System;
using System.Collections.ObjectModel;
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
			DiscoverRequestHandlers(configuration, _assembly);

			if (applyCustomConfiguration)
				ApplyCustomConfiguration(configuration, _assembly);

			return configuration;
		}

		private static void ApplyDefaultConfiguration(HttpHandlerConfiguration configuration)
		{
			configuration.RequestHandlerFactory = new DefaultRequestHandlerFactory();
		}

		private void DiscoverRequestHandlers(HttpHandlerConfiguration configuration, Assembly assembly)
		{
			var requestHandlers = assembly.GetExportedTypes().Where(x => x.GetInterfaces().Contains(typeof(IRequestHandler)));
			configuration.RequestHandlers = new ReadOnlyCollection<Type>(requestHandlers.ToList());
		}

		private static void ApplyCustomConfiguration(HttpHandlerConfiguration configuration, Assembly assembly)
		{
			var bootstrapperTypes = assembly.GetExportedTypes().Where(x => x.GetInterfaces().Contains(typeof(IStartupTask)));

			foreach (var bootstrapperType in bootstrapperTypes)
			{
				var instance = (IStartupTask)Activator.CreateInstance(bootstrapperType);
				instance.Run(configuration);
			}
		}
	}
}