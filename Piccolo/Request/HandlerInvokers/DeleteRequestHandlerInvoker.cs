using System;
using System.Collections.Generic;
using Piccolo.Request.RouteParameterBinders;

namespace Piccolo.Request.HandlerInvokers
{
	public class DeleteRequestHandlerInvoker : BaseRequestHandlerInvoker
	{
		public DeleteRequestHandlerInvoker(Dictionary<Type, IRouteParameterBinder> routeParameterBinders) : base(routeParameterBinders)
		{
		}

		public override string MethodName
		{
			get { return "Delete"; }
		}
	}
}