using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Piccolo.Routing
{
	[DebuggerDisplay("[{RouteTemplateFragment}], {ChildNodes.Count} child node(s)")]
	internal class RouteHandlerLookupNode
	{
		internal RouteHandlerLookupNode() : this(String.Empty, new List<string>(), null)
		{
		}

		internal RouteHandlerLookupNode(string headFragment, IList<string> routeTemplateFragments, Type requestHandler)
		{
			ChildNodes = new List<RouteHandlerLookupNode>();
			IsStaticRouteTemplateFragment = IsStaticFragment(headFragment);
			RouteTemplateFragment = RemoveDynamicFragmentTokens(headFragment);

			if (routeTemplateFragments.Count == 0) // is not a virtual fragment
			{
				RequestHandler = requestHandler;
				RequestHandlerProperties = RouteHandlerDescriptor.GetRequestHandlerProperties(requestHandler);
			}
		}

		internal IList<RouteHandlerLookupNode> ChildNodes { get; set; }
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
				var newChildNode = new RouteHandlerLookupNode(headFragment, remainingTemplateFragments, requestHandler);
				ChildNodes.Add(newChildNode);
				if (remainingTemplateFragments.Any())
					newChildNode.AddNode(remainingTemplateFragments, requestHandler);
			}
		}

		private RouteHandlerLookupNode FindChildNode(string headFragment)
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
	}
}