using System;
using System.Collections.Generic;
using System.Linq;

namespace Piccolo.Routing
{
	internal static class RouteHandlerLookupTreeBuiler
	{
		public static RouteHandlerLookupNode BuildRouteHandlerLookupTree(IEnumerable<Type> requestHandlers)
		{
			var tree = new RouteHandlerLookupNode();
			var knownRouteFragmentSets = new List<IList<string>>();

			foreach (var requestHandler in requestHandlers)
			{
				var routeAttributes = RouteHandlerDescriptor.GetRouteAttributes(requestHandler);
				var routeHandlerVerb = RouteHandlerDescriptor.GetVerb(requestHandler);
				var routeFragmentSets = routeAttributes.Select(x => BuildHandlerIdentifier(routeHandlerVerb, x.Template)).ToList();

				knownRouteFragmentSets.AddRange(routeFragmentSets);
				ScanForUnreachableRouteHandlers(knownRouteFragmentSets, routeAttributes.First().Template, requestHandler);

				foreach (var routeFragementSet in routeFragmentSets)
					tree.AddNode(routeFragementSet, requestHandler);
			}

			return tree;
		}

		private static IList<string> BuildHandlerIdentifier(string verb, string uri)
		{
			var baseHandlerIdentifier = new List<string>(new[] {verb.ToLower(), "_root_"});

			var uriFragments = uri.ToLower().Split(new[] {"/"}, StringSplitOptions.RemoveEmptyEntries);
			if (uriFragments.Length == 0)
				return baseHandlerIdentifier;

			baseHandlerIdentifier.AddRange(uriFragments);
			return baseHandlerIdentifier;
		}

		private static void ScanForUnreachableRouteHandlers(List<IList<string>> routeFragmentSets, string routeTemplate, Type requestHandler)
		{
			var distinctRouteFragmentSets = routeFragmentSets.GroupBy(x => x.Aggregate((a, b) => a + b)).Select(x => x.First());
			if (distinctRouteFragmentSets.Count() != routeFragmentSets.Count())
			{
				string message = string.Format("Handler for route template [{0}] is already defined. Unable to register request handler [{1}] for lookup as it would be unreachable.", routeTemplate, requestHandler.FullName);
				throw new InvalidOperationException(message);
			}
		}
	}
}