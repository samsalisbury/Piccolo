using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

		private static bool IsMatch(Node node, string pathFragment)
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

		internal class Node
		{
			internal Node() : this(string.Empty, new List<string>(), null)
			{
			}

			internal Node(string headFragment, IList<string> routeTemplateFragments, Type requestHandler)
			{
				ChildNodes = new List<Node>();
				IsStaticRouteTemplateFragment = IsStaticFragment(headFragment);
				RouteTemplateFragment = RemoveDynamicFragmentTokens(headFragment);

				if (routeTemplateFragments.Count == 0) // is not a virtual fragment
				{
					RequestHandler = requestHandler;
					RequestHandlerProperties = RouteHandlerDescriptor.GetRequestHandlerProperties(requestHandler);
				}
			}

			internal IList<Node> ChildNodes { get; set; }
			internal bool IsStaticRouteTemplateFragment { get; set; }
			internal string RouteTemplateFragment { get; set; }
			internal Type RequestHandler { get; set; }
			internal Dictionary<string, Type> RequestHandlerProperties { get; set; }

			internal void AddNode(IList<string> routeTemplateFragments, Type requestHandler)
			{
				var headFragment = routeTemplateFragments.First();

				var childNode = FindChildNode(headFragment);
				var remainingTemplateFragments = routeTemplateFragments.Skip(1).ToList();

				if (childNode != null)
				{
					if (remainingTemplateFragments.Count > 0)
					{
						childNode.AddNode(remainingTemplateFragments, requestHandler);
					}
					else
					{
						childNode.RequestHandler = requestHandler;
						childNode.RequestHandlerProperties = RouteHandlerDescriptor.GetRequestHandlerProperties(requestHandler);
					}
				}
				else
				{
					var newChildNode = new Node(headFragment, remainingTemplateFragments, requestHandler);
					ChildNodes.Add(newChildNode);
					if (remainingTemplateFragments.Any())
						newChildNode.AddNode(remainingTemplateFragments, requestHandler);
				}
			}

			private Node FindChildNode(string headFragment)
			{
				return ChildNodes.SingleOrDefault(x => x.RouteTemplateFragment == RemoveDynamicFragmentTokens(headFragment));
			}

			private static bool IsStaticFragment(string headFragment)
			{
				return headFragment.Contains('{') == false;
			}

			private static string RemoveDynamicFragmentTokens(string headFragment)
			{
				return headFragment.Trim(new[] {'{', '}'});
			}

			[ExcludeFromCodeCoverage]
			public override string ToString()
			{
				return string.Format("[{0}], {1} child node(s)", RouteTemplateFragment, ChildNodes.Count);
			}
		}
	}
}