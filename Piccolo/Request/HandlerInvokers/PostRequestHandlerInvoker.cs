using System;
using System.Collections.Generic;
using Piccolo.Request.RouteParameterBinders;

namespace Piccolo.Request.HandlerInvokers
{
	public class PostRequestHandlerInvoker : BaseRequestHandlerInvoker
	{
		public PostRequestHandlerInvoker(Dictionary<Type, IRouteParameterBinder> routeParameterBinders) : base(routeParameterBinders)
		{
		}

		public override string MethodName
		{
			get { return "Post"; }
		}
	}
}