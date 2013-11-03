using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Piccolo.Internal
{
	internal static class ReflectionExtensions
	{
		internal static IList<Type> GetTypesImplementing<TInterface>(this Assembly assembly)
		{
			return assembly.GetExportedTypes().Where(x => x.GetInterfaces().Contains(typeof(TInterface))).ToList();
		}

		internal static TAttribute GetAttribute<TAttribute>(this Type type)
		{
			return (TAttribute)type.GetCustomAttributes(typeof(TAttribute), false).SingleOrDefault();
		}
	}
}