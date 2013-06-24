using System;

namespace Piccolo.Configuration
{
	public interface IRequestHandlerFactory
	{
		IRequestHandler CreateInstance(Type requestHandlerType);
	}
}