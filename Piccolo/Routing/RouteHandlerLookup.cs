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
			_tree = RouteHandlerLookupTreeBuiler.BuildRouteHandlerLookupTree(requestHandlers);
		}

		public RouteHandlerLookupResult FindRequestHandler(string verb, string relativePath)
		{
			var handlerIdentifier = RouteHandlerIdentifierBuiler.BuildRouteHandlerIdentifier(verb, relativePath);
			return FindNode(_tree, handlerIdentifier, new Dictionary<string, string>());
		}

		private static RouteHandlerLookupResult FindNode(RouteHandlerLookupNode node, IList<string> pathFragments, Dictionary<string, string> routeParameters)
		{
			foreach (var childNode in node.ChildNodes)
			{
				if (IsMatch(childNode, pathFragments.First(), routeParameters) == false)
					continue;

				var remainingPathFragments = pathFragments.Skip(1).ToList();
				if (remainingPathFragments.Count == 0)
					return new RouteHandlerLookupResult(childNode.RequestHandler, routeParameters);

				var requestHandler = FindNode(childNode, remainingPathFragments, routeParameters);
				if (requestHandler != null)
					return requestHandler;
			}

			return null;
		}

		private static bool IsMatch(RouteHandlerLookupNode node, string pathFragment, Dictionary<string, string> routeParameters)
		{
			if (node.IsStaticRouteTemplateFragment)
				return node.RouteTemplateFragment == pathFragment;

			if (node.IsVirtualRouteTemplateFragment)
				return true;

			if (node.RequestHandlerPropertyNames.Any(x => x == node.RouteTemplateFragment))
			{
				routeParameters.Add(node.RouteTemplateFragment, pathFragment);
				return true;
			}

			return false;
		}
	}
}