using System;

namespace Piccolo.Configuration
{
	public interface IObjectFactory
	{
		T CreateInstance<T>(Type type);
	}
}