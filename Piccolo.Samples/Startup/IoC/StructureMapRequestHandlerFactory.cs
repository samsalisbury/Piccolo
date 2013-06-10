using System;
using Piccolo.IoC;
using StructureMap;

namespace Piccolo.Samples.Startup.IoC
{
	public class StructureMapRequestHandlerFactory : IRequestHandlerFactory
	{
		public IRequestHandler CreateInstance(Type requestHandlerType)
		{
			return (IRequestHandler)ObjectFactory.GetInstance(requestHandlerType);
		}
	}
}