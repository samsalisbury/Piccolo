using System;
using System.Collections.Generic;
using System.Linq;

namespace Piccolo.UnitTests
{
	public static class Extensions
	{
		public static void Remove<T>(this IList<Type> source)
		{
			source.Remove(source.Single(x => x == typeof(T)));
		}
	}
}