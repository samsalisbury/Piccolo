using System;
using Piccolo.Configuration;
using StructureMap;

namespace Piccolo.Tasks.Startup.IoC
{
	public class StructureMapObjectFactory : IObjectFactory
	{
		public T CreateInstance<T>(Type requestHandlerType)
		{
			return (T)ObjectFactory.GetInstance(requestHandlerType);
		}
	}
}