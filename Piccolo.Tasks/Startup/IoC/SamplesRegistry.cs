using Piccolo.Tasks.Repositories;
using StructureMap.Configuration.DSL;

namespace Piccolo.Tasks.Startup.IoC
{
	public class SamplesRegistry : Registry
	{
		public SamplesRegistry()
		{
			For<ITaskRepository>().Singleton().Use<InMemoryTaskRepository>();
		}
	}
}