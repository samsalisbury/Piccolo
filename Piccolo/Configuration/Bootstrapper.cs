﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Piccolo.IoC;
using Piccolo.Routing;

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

			DiscoverRequestHandlers(configuration, _assembly);
			ApplyDefaultConfiguration(configuration);

			if (applyCustomConfiguration)
				RunStartupTasks(configuration, _assembly);

			return configuration;
		}

		private static void DiscoverRequestHandlers(HttpHandlerConfiguration configuration, Assembly assembly)
		{
			var requestHandlers = assembly.GetExportedTypes().Where(x => x.GetInterfaces().Contains(typeof(IRequestHandler)));
			configuration.RequestHandlers = new ReadOnlyCollection<Type>(requestHandlers.ToList());
		}

		private static void ApplyDefaultConfiguration(HttpHandlerConfiguration configuration)
		{
			configuration.RequestHandlerFactory = new DefaultRequestHandlerFactory();
			configuration.Router = new RequestRouter(configuration);
		}

		private static void RunStartupTasks(HttpHandlerConfiguration configuration, Assembly assembly)
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