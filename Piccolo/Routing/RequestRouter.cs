using System;
using System.Collections.Generic;
using System.Linq;

namespace Piccolo.Routing
{
	public class RequestRouter
	{
		private readonly RouteHandlerLookupNode _tree;

		public RequestRouter(IEnumerable<Type> requestHandlers)
		{
			_tree = RouteHandlerLookupTreeBuilder.BuildRouteHandlerLookupTree(requestHandlers);
		}

		public RouteHandlerLookupResult FindRequestHandler(string verb, string applicationPath, Uri uri)
		{
			var applicationRelativePath = uri.AbsolutePath.Remove(0, applicationPath.Length);
			var handlerIdentifier = RouteIdentifierBuilder.BuildIdentifier(verb, applicationRelativePath);
			
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

			return RouteHandlerLookupResult.FailedResult;
		}

		private static bool IsMatch(RouteHandlerLookupNode node, string pathFragment, Dictionary<string, string> routeParameters)
		{
			if (node.IsStaticRouteTemplateFragment)
				return node.RouteTemplateFragment.Equals(pathFragment, StringComparison.InvariantCultureIgnoreCase);

			routeParameters.Add(node.RouteTemplateFragment, pathFragment);
			return true;
		}
	}
}