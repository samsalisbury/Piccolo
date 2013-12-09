using System;
using Piccolo.Configuration;
using StructureMap;

namespace Piccolo.Tasks.Startup.IoC
{
	public class StructureMapObjectFactory : IObjectFactory
	{
		public T CreateInstance<T>(Type type)
		{
			return (T)ObjectFactory.GetInstance(type);
		}
	}
}