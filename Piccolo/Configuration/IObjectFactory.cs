using System;

namespace Piccolo.Configuration
{
	public interface IObjectFactory
	{
		IRequestHandler CreateInstance(Type requestHandlerType);
	}
}