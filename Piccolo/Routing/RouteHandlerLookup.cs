using System;
using System.Collections.Generic;
using System.Linq;

namespace Piccolo.Routing
{
	public class RouteHandlerLookup
	{
		private readonly RouteHandlerLookupNode _tree;

		public RouteHandlerLookup(IEnumerable<Type> requestHandlers)
		{
			_tree = new RouteHandlerLookupNode();
			var knownRouteFragmentSets = new List<IList<string>>();

			foreach (var requestHandler in requestHandlers)
			{
				var routeAttributes = RouteHandlerDescriptor.GetRouteAttributes(requestHandler);
				var routeHandlerVerb = RouteHandlerDescriptor.GetVerb(requestHandler);
				var routeFragmentSets = routeAttributes.Select(x => BuildHandlerIdentifier(routeHandlerVerb, x.Template)).ToList();

				knownRouteFragmentSets.AddRange(routeFragmentSets);
				ScanForUnreachableRouteHandlers(knownRouteFragmentSets, routeAttributes.First().Template, requestHandler);

				foreach (var routeFragementSet in routeFragmentSets)
					_tree.AddNode(routeFragementSet, requestHandler);
			}
		}

		public Type FindRequestHandler(string verb, string relativePath)
		{
			var handlerIdentifier = BuildHandlerIdentifier(verb, relativePath);
			return FindNode(_tree, handlerIdentifier);
		}

		private static Type FindNode(RouteHandlerLookupNode node, IList<string> pathFragments)
		{
			foreach (var childNode in node.ChildNodes)
			{
				if (IsMatch(childNode, pathFragments.First()) == false)
					continue;

				var remainingPathFragments = pathFragments.Skip(1).ToList();

				foreach (var grandChildNode in childNode.ChildNodes)
				{
					var requestHandler = Lookahead(grandChildNode, remainingPathFragments);
					if (requestHandler != null)
						return requestHandler;
				}
			}

			return null;
		}

		private static Type Lookahead(RouteHandlerLookupNode node, IList<string> pathFragments)
		{
			if (IsMatch(node, pathFragments.First()) == false)
				return null;

			var remainingPathFragments = pathFragments.Skip(1).ToList();
			if (remainingPathFragments.Count == 0)
				return node.RequestHandler;

			foreach (var childNode in node.ChildNodes)
			{
				var requestHandler = Lookahead(childNode, remainingPathFragments);
				if (requestHandler != null)
					return requestHandler;
			}

			return null;
		}

		private static bool IsMatch(RouteHandlerLookupNode node, string pathFragment)
		{
			if (node.IsStaticRouteTemplateFragment)
				return node.RouteTemplateFragment == pathFragment;

			if (node.IsStaticRouteTemplateFragment == false && node.ChildNodes.Count > 0 && node.RequestHandlerProperties == null) // IsVirtualNode
				return true;

			Type propertyType;
			if (node.RequestHandlerProperties.TryGetValue(node.RouteTemplateFragment, out propertyType) == false)
				return false;

			return FragmentTypeMatchesPropertyType(pathFragment, propertyType);
		}

		private static bool FragmentTypeMatchesPropertyType(string pathFragment, Type propertyType)
		{
			// TODO: Add support for other datatypes
			int result;
			return int.TryParse(pathFragment, out result);
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