using System;

namespace Piccolo.Configuration
{
	public class DefaultObjectFactory : IObjectFactory
	{
		public T CreateInstance<T>(Type type)
		{
			return (T)Activator.CreateInstance(type);
		}
	}
}