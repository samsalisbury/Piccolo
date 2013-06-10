using System;

namespace Piccolo.IoC
{
	public class DefaultRequestHandlerFactory : IRequestHandlerFactory
	{
		public IRequestHandler CreateInstance(Type requestHandlerType)
		{
			return (IRequestHandler)Activator.CreateInstance(requestHandlerType);
		}
	}
}