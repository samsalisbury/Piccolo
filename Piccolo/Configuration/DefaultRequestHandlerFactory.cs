using System;

namespace Piccolo.Configuration
{
	public class DefaultRequestHandlerFactory : IRequestHandlerFactory
	{
		public IRequestHandler CreateInstance(Type requestHandlerType)
		{
			return (IRequestHandler)Activator.CreateInstance(requestHandlerType);
		}
	}
}