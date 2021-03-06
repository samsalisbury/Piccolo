using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Piccolo.Internal
{
	internal static class ReflectionExtensions
	{
		private const BindingFlags MemberLookupFlags = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public;

		internal static IList<Type> GetTypesImplementing<TInterface>(this Assembly assembly)
		{
			return assembly.GetExportedTypes().Where(x => x.GetInterfaces().Contains(typeof(TInterface))).ToList();
		}

		internal static IList<PropertyInfo> GetPropertiesDecoratedWith<TAttribute>(this Type type) where TAttribute : Attribute
		{
			return type.GetProperties().Where(x => x.GetCustomAttributes(typeof(TAttribute), true).Any()).ToList();
		}

		internal static TAttribute GetAttribute<TAttribute>(this Type type)
		{
			return (TAttribute)type.GetCustomAttributes(typeof(TAttribute), false).SingleOrDefault();
		}

		internal static TAttribute GetAttribute<TAttribute>(this PropertyInfo property)
		{
			return (TAttribute)property.GetCustomAttributes(typeof(TAttribute), false).SingleOrDefault();
		}

		internal static IList<TAttribute> GetAttributes<TAttribute>(this Type type)
		{
			return type.GetCustomAttributes(typeof(TAttribute), true).Cast<TAttribute>().ToList();
		}

		internal static Type GetMethodParameterType(this Type type, string methodName)
		{
			var parameter = type.GetMethod(methodName, MemberLookupFlags).GetParameters().FirstOrDefault();
			return parameter == null ? null : parameter.ParameterType;
		}

		internal static TResponse InvokeMethod<TResponse>(this object target, string methodName, object parameter)
		{
			var method = target.GetType().GetMethod(methodName, MemberLookupFlags);
			return (TResponse)method.Invoke(target, parameter == null ? null : new[] {parameter});
		}

		internal static TValue GetPropertyValue<TValue>(this object target, string propertyName)
		{
			return (TValue)target
				.GetType()
				.FindProperty(propertyName)
				.GetValue(target, null);
		}

		internal static PropertyInfo FindProperty(this Type type, string propertyName)
		{
			return type.GetProperty(propertyName, MemberLookupFlags);
		}
	}
}