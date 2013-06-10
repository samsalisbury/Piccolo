using System;

namespace Piccolo.IoC
{
	public interface IRequestHandlerFactory
	{
		IRequestHandler CreateInstance(Type requestHandlerType);
	}
}