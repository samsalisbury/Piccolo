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

		public Type FindRequestHandler(string verb, string relativePath)
		{
			var handlerIdentifier = RouteHandlerIdentifierBuiler.BuildRouteHandlerIdentifier(verb, relativePath);
			return FindNode(_tree, handlerIdentifier);
		}

		private static Type FindNode(RouteHandlerLookupNode node, IList<string> pathFragments)
		{
			foreach (var childNode in node.ChildNodes)
			{
				if (IsMatch(childNode, pathFragments.First()) == false)
					continue;

				var remainingPathFragments = pathFragments.Skip(1).ToList();
				if (remainingPathFragments.Count == 0)
					return childNode.RequestHandler;

				var requestHandler = FindNode(childNode, remainingPathFragments);
				if (requestHandler != null)
					return requestHandler;
			}

			return null;
		}

		private static bool IsMatch(RouteHandlerLookupNode node, string pathFragment)
		{
			if (node.IsStaticRouteTemplateFragment)
				return node.RouteTemplateFragment == pathFragment;

			if (node.IsVirtualRouteTemplateFragment)
				return true;

			Type propertyType;
			return node.RequestHandlerProperties.TryGetValue(node.RouteTemplateFragment, out propertyType);
		}
	}
}