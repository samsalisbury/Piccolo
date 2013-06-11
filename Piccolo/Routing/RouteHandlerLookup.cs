using System;
using System.Collections.Generic;
using System.Linq;

namespace Piccolo.Routing
{
	internal class RouteHandlerLookup
	{
		private readonly Node _tree;

		public RouteHandlerLookup(IEnumerable<Type> requestHandlers)
		{
			_tree = new Node();

			foreach (var requestHandler in requestHandlers)
			{
				var routeAttributes = requestHandler.GetCustomAttributes(typeof(RouteAttribute), true);
				var routeUris = routeAttributes.Cast<RouteAttribute>().Select(x => Split(x.Uri));
				foreach (var routeUri in routeUris)
					_tree.AddChildNode(routeUri, requestHandler);
			}
		}

		public Type FindRequestHandlerForPath(string absolutePath)
		{
			var pathFragments = Split(absolutePath);
			return FindNode(_tree, pathFragments);
		}

		private static Type FindNode(Node node, IList<string> pathFragments)
		{
			foreach (var childNode in node.ChildNodes)
			{
				if (IsMatch(childNode, pathFragments.First()))
				{
					var remainingPathFragments = pathFragments.Skip(1).ToList();
					return remainingPathFragments.Any() ? FindNode(childNode, remainingPathFragments) : childNode.RequestHandler;
				}

				return null;
			}

			return null;
		}

		private static bool IsMatch(Node node, string pathFragment)
		{
			// is static and matches
			if (node.IsStaticRouteTemplateFragment && node.RouteTemplateFragment == pathFragment)
				return true;

			// ## the rest handles dynamic fragments

			// property for fragment name does not exist
			Type propertyType;
			if (node.RequestHandlerProperties.TryGetValue(node.RouteTemplateFragment, out propertyType) == false)
				return false;

			// try to parse fragment as int
			int result;
			return int.TryParse(pathFragment, out result);
		}

		private static IList<string> Split(string uri)
		{
			return uri.ToLower().Split(new[] {"/"}, StringSplitOptions.RemoveEmptyEntries);
		}

		internal class Node
		{
			private static readonly string[] _reservedPropertyNames = new[] {"output"};

			internal Node() : this(string.Empty, new List<string>(), null, new Dictionary<string, Type>())
			{
			}

			internal Node(string headFragment, IList<string> routeTemplateFragments, Type requestHandler, Dictionary<string, Type> requestHandlerProperties)
			{
				ChildNodes = new List<Node>();
				IsStaticRouteTemplateFragment = headFragment.Contains('{') == false;
				RouteTemplateFragment = headFragment.Trim(new[] {'{', '}'});
				RequestHandler = requestHandler;
				RequestHandlerProperties = requestHandlerProperties;

				if (routeTemplateFragments.Any())
					AddChildNode(routeTemplateFragments, requestHandler);
			}

			internal IList<Node> ChildNodes { get; set; }
			internal bool IsStaticRouteTemplateFragment { get; set; }
			internal string RouteTemplateFragment { get; set; }
			internal Type RequestHandler { get; set; }
			internal Dictionary<string, Type> RequestHandlerProperties { get; set; }

			internal void AddChildNode(IList<string> routeTemplateFragments, Type requestHandler)
			{
				var headFragment = routeTemplateFragments.First();
				var childNode = FindChildNode(headFragment);
				var remainingTemplateFragments = routeTemplateFragments.Skip(1).ToList();

				if (childNode != null)
				{
					childNode.AddChildNode(remainingTemplateFragments, requestHandler);
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
}