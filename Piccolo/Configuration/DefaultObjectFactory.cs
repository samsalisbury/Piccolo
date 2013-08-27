using System;

namespace Piccolo.Configuration
{
	public class DefaultObjectFactory : IObjectFactory
	{
		public T CreateInstance<T>(Type requestHandlerType)
		{
			return (T)Activator.CreateInstance(requestHandlerType);
		}
	}
}