using System;
using Piccolo.Configuration;
using StructureMap;

namespace Piccolo.Tasks.Startup.IoC
{
	public class StructureMapObjectFactory : IObjectFactory
	{
		public IRequestHandler CreateInstance(Type requestHandlerType)
		{
			return (IRequestHandler)ObjectFactory.GetInstance(requestHandlerType);
		}
	}
}