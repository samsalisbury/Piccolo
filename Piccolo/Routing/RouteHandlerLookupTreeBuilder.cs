using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Piccolo.Routing
{
	internal static class RouteHandlerLookupTreeBuilder
	{
		internal static RouteHandlerLookupNode BuildRouteHandlerLookupTree(IEnumerable<Type> requestHandlers)
		{
			var tree = new RouteHandlerLookupNode();
			var knownRouteFragmentSets = new List<IList<string>>();

			foreach (var requestHandler in requestHandlers)
			{
				var routeAttributes = RequestHandlerDescriptor.GetRouteAttributes(requestHandler);
				var routeHandlerVerb = RequestHandlerDescriptor.GetVerb(requestHandler);
				var routeFragmentSets = routeAttributes.Select(x => RouteIdentifierBuiler.BuildIdentifier(routeHandlerVerb, x.Template)).ToList();

				knownRouteFragmentSets.AddRange(routeFragmentSets);
				ScanForUnreachableRouteHandlers(knownRouteFragmentSets, routeAttributes.First().Template, requestHandler);
				ScanForUnreachableRouteParameters(routeFragmentSets, routeAttributes, requestHandler);

				foreach (var routeFragementSet in routeFragmentSets)
					tree.AddNode(routeFragementSet, requestHandler);
			}

			return tree;
		}

		private static void ScanForUnreachableRouteHandlers(List<IList<string>> routeFragmentSets, string routeTemplate, Type requestHandler)
		{
			var distinctRouteFragmentSets = routeFragmentSets.GroupBy(x => x.Aggregate((a, b) => a + b)).Select(x => x.First());
			if (distinctRouteFragmentSets.Count() != routeFragmentSets.Count())
			{
				var message = string.Format("Handler for route template [{0}] is already defined. Unable to register request handler [{1}] for lookup as it would be unreachable.", routeTemplate, requestHandler.FullName);
				throw new InvalidOperationException(message);
			}
		}

		private static void ScanForUnreachableRouteParameters(IEnumerable<IList<string>> routeFragmentSets, IEnumerable<RouteAttribute> routeAttributes, Type requestHandlerType)
		{
			var dynamicRouteFragments = (from routeFragmentSet in routeFragmentSets
				from fragment in routeFragmentSet
				where RouteHandlerLookupNode.IsStaticFragment(fragment) == false
				select RouteHandlerLookupNode.RemoveDynamicFragmentTokens(fragment)).ToList();

			if (dynamicRouteFragments.Count == 0)
				return;

			var propertyNames = requestHandlerType.GetProperties().Select(x => x.Name).ToList();

			var unreachableParameter = dynamicRouteFragments.FirstOrDefault(f => propertyNames.Any(n => n.Equals(f, StringComparison.InvariantCultureIgnoreCase)) == false);
			if (unreachableParameter == null)
				return;

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

			throw new InvalidOperationException(messageBuilder.ToString());
		}
	}
}