using System;
using System.Collections.Generic;
using System.Linq;

namespace Piccolo.Routing
{
	public class RequestRouter : IRequestRouter
	{
		private readonly RouteHandlerLookupNode _tree;

		public RequestRouter(IEnumerable<Type> requestHandlers)
		{
			_tree = RouteHandlerLookupTreeBuilder.BuildRouteHandlerLookupTree(requestHandlers);
		}

		public RouteHandlerLookupResult FindRequestHandler(string verb, Uri uri)
		{
			var handlerIdentifier = RouteIdentifierBuiler.BuildIdentifier(verb, uri.AbsolutePath);
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
				return node.RouteTemplateFragment.Equals(pathFragment, StringComparison.InvariantCultureIgnoreCase);

			if (node.IsVirtualRouteTemplateFragment)
				return true;

			routeParameters.Add(node.RouteTemplateFragment, pathFragment);
			return true;
		}
	}
}