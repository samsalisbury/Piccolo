using Piccolo.Samples.Repositories;
using StructureMap.Configuration.DSL;

namespace Piccolo.Samples.Startup.IoC
{
	public class SamplesRegistry : Registry
	{
		public SamplesRegistry()
		{
			For<ITaskRepository>().Singleton().Use<InMemoryTaskRepository>();
		}
	}
}