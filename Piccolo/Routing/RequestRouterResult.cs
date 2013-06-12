using System;

namespace Piccolo.Routing
{
	public class RequestRouterResult
	{
		public RequestRouterResult(Type handlerType)
		{
			HandlerType = handlerType;
		}

		public Type HandlerType { get; private set; }
	}
}