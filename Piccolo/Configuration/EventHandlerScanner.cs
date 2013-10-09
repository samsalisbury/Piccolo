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
			var requestProcessingEventHandlers = assembly.GetExportedTypes().Where(x => x.GetInterfaces().Contains(typeof(IHandle<TEvent>)));
			return requestProcessingEventHandlers.ToList();
		}
	}
}