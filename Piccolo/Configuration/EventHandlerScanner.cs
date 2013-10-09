using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Piccolo.Events;

namespace Piccolo.Configuration
{
	internal static class EventHandlerScanner
	{
		internal static List<Type> FindEventHandlersForEvent<TEvent>(Assembly assembly) where TEvent : IEvent
		{
			return assembly
				.GetExportedTypes()
				.Where(x => x.GetInterfaces().Contains(typeof(IHandle<TEvent>)))
				.OrderBy(x =>
				{
					var priorityAttribute = x.GetCustomAttributes(typeof(PriorityAttribute), false).FirstOrDefault();
					return priorityAttribute != null ? ((PriorityAttribute)priorityAttribute).Level : byte.MaxValue;
				})
				.ToList();
		}
	}
}