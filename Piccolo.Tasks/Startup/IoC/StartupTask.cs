using Piccolo.Configuration;
using StructureMap;

namespace Piccolo.Tasks.Startup.IoC
{
	public class StartupTask : IStartupTask
	{
		public void Run(PiccoloConfiguration configuration)
		{
			ObjectFactory.Initialize(c => c.IncludeRegistry<SamplesRegistry>());

			configuration.RequestHandlerFactory = new StructureMapRequestHandlerFactory();
		}
	}
}