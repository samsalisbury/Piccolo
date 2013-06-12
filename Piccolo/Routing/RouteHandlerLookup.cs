using System;
using System.Collections.Generic;
using System.Linq;

namespace Piccolo.Routing
{
	public class RouteHandlerLookup
	{
		private readonly Node _tree;

		public RouteHandlerLookup(IEnumerable<Type> requestHandlers)
		{
			_tree = new Node();
			var knownRouteFragmentSets = new List<IList<string>>();

			foreach (var requestHandler in requestHandlers)
			{
				var routeAttributes = RouteHandlerDescriptor.GetRouteAttributes(requestHandler);
				var routeHandlerVerb = RouteHandlerDescriptor.GetVerb(requestHandler);
				var routeFragmentSets = routeAttributes.Select(x => BuildHandlerIdentifier(routeHandlerVerb, x.Uri)).ToList();

				knownRouteFragmentSets.AddRange(routeFragmentSets);
				ScanForUnreachableRouteHandlers(knownRouteFragmentSets, routeAttributes.First().Uri, requestHandler);

				foreach (var routeFragementSet in routeFragmentSets)
					_tree.AddNode(routeFragementSet, requestHandler);
			}
		}

		public Type FindRequestHandler(string verb, string relativePath)
		{
			var handlerIdentifier = BuildHandlerIdentifier(verb, relativePath);
			return FindNode(_tree, handlerIdentifier);
		}

		private static Type FindNode(Node node, IList<string> pathFragments)
		{
			if (IsRootMatch(node, pathFragments))
				return node.RootNode.RequestHandler;

			foreach (var childNode in node.ChildNodes)
			{
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

		private static Type Lookahead(Node node, IList<string> pathFragments)
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

		private static bool IsRootMatch(Node node, ICollection<string> pathFragments)
		{
			return pathFragments.Count == 1 && pathFragments.First() == node.RootNode.RouteTemplateFragment;
		}

		private static bool IsMatch(Node node, string pathFragment)
		{
			if (node.IsStaticRouteTemplateFragment && node.RouteTemplateFragment == pathFragment)
				return true;

			Type propertyType;
			if (node.RequestHandlerProperties.TryGetValue(node.RouteTemplateFragment, out propertyType) == false)
				return false;

			return FragmentTypeMatchesPropertyType(pathFragment, propertyType);
		}

		private static bool FragmentTypeMatchesPropertyType(string pathFragment, Type propertyType)
		{
			int result;
			return int.TryParse(pathFragment, out result);
		}

		private static IList<string> BuildHandlerIdentifier(string verb, string uri)
		{
			var uriFragments = uri.ToLower().Split(new[] {"/"}, StringSplitOptions.RemoveEmptyEntries);

			var handlerIdentifier = new List<string>(new[] {verb.ToLower()});
			handlerIdentifier.AddRange(uriFragments);

			return uriFragments.Length == 0 ? new List<string>(new[] {string.Empty}) : handlerIdentifier;
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

		internal class Node
		{
			internal Node() : this(string.Empty, new List<string>(), null)
			{
				RootNode = this;
			}

			internal Node(string headFragment, IList<string> routeTemplateFragments, Type requestHandler)
			{
				ChildNodes = new List<Node>();
				IsStaticRouteTemplateFragment = IsStaticFragment(headFragment);
				RouteTemplateFragment = RemoveDynamicFragmentTokens(headFragment);
				RequestHandler = requestHandler;
				RequestHandlerProperties = RouteHandlerDescriptor.GetRequestHandlerProperties(requestHandler);

				if (routeTemplateFragments.Any())
					AddNode(routeTemplateFragments, requestHandler);
			}

			internal Node RootNode { get; set; }
			internal IList<Node> ChildNodes { get; set; }
			internal bool IsStaticRouteTemplateFragment { get; set; }
			internal string RouteTemplateFragment { get; set; }
			internal Type RequestHandler { get; set; }
			internal Dictionary<string, Type> RequestHandlerProperties { get; set; }

			internal void AddNode(IList<string> routeTemplateFragments, Type requestHandler)
			{
				var headFragment = routeTemplateFragments.First();

				if (IsRootNodeFragment(headFragment))
				{
					RequestHandler = requestHandler;
					RequestHandlerProperties = RouteHandlerDescriptor.GetRequestHandlerProperties(requestHandler);
					return;
				}

				var childNode = FindChildNode(headFragment);
				var remainingTemplateFragments = routeTemplateFragments.Skip(1).ToList();

				if (childNode != null)
					childNode.AddNode(remainingTemplateFragments, requestHandler);
				else
					ChildNodes.Add(new Node(headFragment, remainingTemplateFragments, requestHandler));
			}

			private static bool IsRootNodeFragment(string headFragment)
			{
				return headFragment == string.Empty;
			}

			private Node FindChildNode(string headFragment)
			{
				return ChildNodes.SingleOrDefault(x => x.RouteTemplateFragment == headFragment);
			}

			private static bool IsStaticFragment(string headFragment)
			{
				return headFragment.Contains('{') == false;
			}

			private static string RemoveDynamicFragmentTokens(string headFragment)
			{
				return headFragment.Trim(new[] {'{', '}'});
			}
		}
	}
}