using System;
using System.Collections.Generic;
using Piccolo.Configuration;

namespace Piccolo.Events
{
	public class EventDispatcher
	{
		private readonly EventHandlers _eventHandlers;

		public EventDispatcher(EventHandlers eventHandlers)
		{
			_eventHandlers = eventHandlers;
		}

		public void RaiseRequestProcessingEvent(PiccoloContext context)
		{
			RaiseEvent<RequestProcessingEvent>(_eventHandlers.RequestProcessing, context);
		}

		private static void RaiseEvent<TEvent>(IEnumerable<Type> eventHandlers, PiccoloContext context) where TEvent : IEvent, new()
		{
			var args = new TEvent {Context = context};

			foreach (var eventHandlerType in eventHandlers)
			{
				var eventHandler = (IHandle<TEvent>)Activator.CreateInstance(eventHandlerType);
				eventHandler.Handle(args);

				if (args.StopProcessing)
					break;
			}
		}
	}
}