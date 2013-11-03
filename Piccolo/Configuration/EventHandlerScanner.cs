using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Piccolo.Events;
using Piccolo.Internal;

namespace Piccolo.Configuration
{
	internal static class EventHandlerScanner
	{
		internal static List<Type> FindEventHandlersForEvent<TEvent>(Assembly assembly) where TEvent : IEvent
		{
			return assembly
				.GetTypesImplementing<IHandle<TEvent>>()
				.OrderBy(type =>
				{
					var priorityAttribute = type.GetAttribute<PriorityAttribute>();
					return priorityAttribute != null ? priorityAttribute.Level : byte.MaxValue;
				})
				.ToList();
		}
	}
}