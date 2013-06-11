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

			foreach (var requestHandler in requestHandlers)
			{
				var routeAttributes = RouteHandlerDescriptor.GetRouteAttributes(requestHandler);
				var routeFragmentSets = routeAttributes.Select(x => GetPathFragments(x.Uri));

				foreach (var routeFragementSet in routeFragmentSets)
					_tree.AddNode(routeFragementSet, requestHandler);
			}
		}

		public Type FindRequestHandlerForPath(string relativePath)
		{
			var pathFragments = GetPathFragments(relativePath);
			return FindNode(_tree, pathFragments);
		}

		private static Type FindNode(Node node, IList<string> pathFragments)
		{
			if (IsRootMatch(node, pathFragments))
				return node.RootNode.RequestHandler;

			foreach (var childNode in node.ChildNodes)
			{
				if (IsMatch(childNode, pathFragments.First()) == false)
					continue;

				var remainingPathFragments = pathFragments.Skip(1).ToList();
				if (remainingPathFragments.Count == 0)
					return childNode.RequestHandler;

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

		private static IList<string> GetPathFragments(string uri)
		{
			var fragments = uri.ToLower().Split(new[] {"/"}, StringSplitOptions.RemoveEmptyEntries);
			return fragments.Length == 0 ? new[] {string.Empty} : fragments;
		}

		internal class Node
		{
			private static readonly string[] _reservedPropertyNames = new[] {"output"};

			internal Node() : this(string.Empty, new List<string>(), null, new Dictionary<string, Type>())
			{
				RootNode = this;
			}

			internal Node(string headFragment, IList<string> routeTemplateFragments, Type requestHandler, Dictionary<string, Type> requestHandlerProperties)
			{
				ChildNodes = new List<Node>();
				IsStaticRouteTemplateFragment = headFragment.Contains('{') == false;
				RouteTemplateFragment = headFragment.Trim(new[] {'{', '}'});
				RequestHandler = requestHandler;
				RequestHandlerProperties = requestHandlerProperties;

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

				// matches root
				if (headFragment == string.Empty)
				{
					RequestHandler = requestHandler;
					RequestHandlerProperties = GetRequestHandlerProperties(requestHandler);
					return;
				}

				var childNode = FindChildNode(headFragment);
				var remainingTemplateFragments = routeTemplateFragments.Skip(1).ToList();

				if (childNode != null)
				{
					childNode.AddNode(remainingTemplateFragments, requestHandler);
				}
				else
				{
					var requestHandlerProperties = GetRequestHandlerProperties(requestHandler);
					ChildNodes.Add(new Node(headFragment, remainingTemplateFragments, requestHandler, requestHandlerProperties));
				}
			}

			private static Dictionary<string, Type> GetRequestHandlerProperties(Type requestHandler)
			{
				var allProperties = requestHandler.GetProperties();
				var allInputProperties = allProperties.Where(x => _reservedPropertyNames.Contains(x.Name.ToLower()) == false);

				return allInputProperties.ToDictionary(x => x.Name.ToLower(), x => x.PropertyType);
			}

			private Node FindChildNode(string headFragment)
			{
				return ChildNodes.SingleOrDefault(x => x.RouteTemplateFragment == headFragment);
			}
		}
	}

	public static class RouteHandlerDescriptor
	{
		public static List<RouteAttribute> GetRouteAttributes(Type requestHandler)
		{
			return requestHandler.GetCustomAttributes(typeof(RouteAttribute), true).Cast<RouteAttribute>().ToList();
		}
	}
}