using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Piccolo.Validation;

namespace Piccolo.Internal
{
	internal class ExceptionMessageBuilder
	{
		internal static string BuildDuplicateRequestHandlerMessage(string routeTemplate, Type requestHandler)
		{
			return string.Format("Handler for route template [{0}] is already defined. Unable to register request handler [{1}] for lookup as it would be unreachable.", routeTemplate, requestHandler.FullName);
		}

		internal static string BuildUnreachableRouteParameterMessage(IEnumerable<RouteAttribute> routeAttributes, Type requestHandler, string unreachableParameter, List<string> propertyNames)
		{
			var messageBuilder = new StringBuilder();
			messageBuilder.Append("Unreachable route parameter detected: ");
			messageBuilder.AppendFormat("request handler [{0}] does not expose property [{1}].", requestHandler.FullName, unreachableParameter);
			messageBuilder.AppendLine();
			messageBuilder.AppendLine();
			messageBuilder.AppendLine("Route Templates:");
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
			return string.Format("Request handler [{0}] does not implement any of the following supported interfaces: IGet<TOutput>, IPut<TInput, TOutput>, IPost<TInput, TOutput>, IDelete<TInput, TOutput>.", requestHandler.FullName);
		}

		internal static string BuildUnsupportedParameterTypeMessage(PropertyInfo property)
		{
			return string.Format("Parameter [{0}.{1}] is of type {2} which is not supported.. Supported types are:" +
			                     "{3} - System.String" +
			                     "{3} - System.Boolean" +
			                     "{3} - System.Boolean?" +
			                     "{3} - System.Int32" +
			                     "{3} - System.Int32?" +
			                     "{3} - System.DateTime" +
			                     "{3} - System.DateTime?", property.DeclaringType.FullName, property.Name, property.PropertyType, Environment.NewLine);
		}

		internal static string BuildInvalidParameterAssignmentMessage(PropertyInfo property, string value)
		{
			return string.Format("Value \"{0}\" could not be assigned to parameter [{1}.{2}] of type {3}.", value, property.DeclaringType.FullName, property.Name, property.PropertyType);
		}

		internal static string BuildMissingGlobalAsaxMessage()
		{
			return "Unable to auto-configure Piccolo: Global.asax could not be found (see https://github.com/opentable/Piccolo/blob/master/Piccolo/PiccoloHttpHandler.cs#L15 for more information).";
		}

		internal static string BuildMissingRouteMessage(Type requestHandler)
		{
			return string.Format("Request handler [{0}] does not have any routes defined.", requestHandler);
		}

		public static string BuildInvalidPayloadValidatorMessage(Type payloadValidator, Type requestHandler)
		{
			return string.Format("Payload validator [{0}] defined on request handler [{1}] does not implement interface {2}.", payloadValidator, requestHandler, typeof(IPayloadValidator<>));
		}
	}
}