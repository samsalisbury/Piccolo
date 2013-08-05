using System;
using Piccolo.Configuration;
using StructureMap;

namespace Piccolo.Tasks.Startup.IoC
{
	public class StructureMapRequestHandlerFactory : IRequestHandlerFactory
	{
		public IRequestHandler CreateInstance(Type requestHandlerType)
		{
			return (IRequestHandler)ObjectFactory.GetInstance(requestHandlerType);
		}
	}
}