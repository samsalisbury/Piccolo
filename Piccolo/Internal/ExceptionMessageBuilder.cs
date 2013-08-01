using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Piccolo.Internal
{
	internal class ExceptionMessageBuilder
	{
		internal static string BuildDuplicateRequestHandlerMessage(string routeTemplate, Type requestHandler)
		{
			return string.Format("Handler for route template [{0}] is already defined. Unable to register request handler [{1}] for lookup as it would be unreachable.", routeTemplate, requestHandler.FullName);
		}

		internal static string BuildUnreachableRouteParameterMessage(IEnumerable<RouteAttribute> routeAttributes, Type requestHandlerType, string unreachableParameter, List<string> propertyNames)
		{
			var messageBuilder = new StringBuilder();
			messageBuilder.Append("Unreachable route parameter detected: ");
			messageBuilder.AppendFormat("request handler [{0}] does not expose property {1}.", requestHandlerType.FullName, unreachableParameter);
			messageBuilder.AppendLine();
			messageBuilder.AppendLine();
			messageBuilder.AppendLine("Routes Templates:");
			foreach (var routeTemplate in routeAttributes.Select(x => x.Template))
			{
				messageBuilder.AppendLine(string.Format(" - {0}", routeTemplate));
			}
			messageBuilder.AppendLine();
			messageBuilder.AppendLine(string.Format("Number of public instance properties found: {0}", propertyNames.Count));
			foreach (var propertyName in propertyNames)
			{
				messageBuilder.AppendLine(string.Format(" - {0}", propertyName));
			}

			return messageBuilder.ToString();
		}

		internal static string BuildInvalidRequestHandlerImplementationMessage(Type requestHandler)
		{
			return string.Format("Request handler [{0}] does not implement any of the following supported interfaces: IGet<T>, IPut<T>, IPost<T>, IDelete.", requestHandler.FullName);
		}
	}
}