﻿using Piccolo.Configuration;
using StructureMap;

namespace Piccolo.Samples.Startup.IoC
{
	public class StartupTask : IStartupTask
	{
		public void Run(HttpHandlerConfiguration configuration)
		{
			ObjectFactory.Initialize(c => c.IncludeRegistry<SamplesRegistry>());

			configuration.RequestHandlerFactory = new StructureMapRequestHandlerFactory();
		}
	}
}