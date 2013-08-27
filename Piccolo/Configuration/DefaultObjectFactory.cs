using System;

namespace Piccolo.Configuration
{
	public class DefaultObjectFactory : IObjectFactory
	{
		public IRequestHandler CreateInstance(Type requestHandlerType)
		{
			return (IRequestHandler)Activator.CreateInstance(requestHandlerType);
		}
	}
}