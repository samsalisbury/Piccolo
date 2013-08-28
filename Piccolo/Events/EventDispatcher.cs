using System;
using System.Collections.Generic;
using Piccolo.Configuration;

namespace Piccolo.Events
{
	public class EventDispatcher
	{
		private readonly EventHandlers _eventHandlers;
		private readonly IObjectFactory _objectFactory;

		public EventDispatcher(EventHandlers eventHandlers, IObjectFactory objectFactory)
		{
			_eventHandlers = eventHandlers;
			_objectFactory = objectFactory;
		}

		public void RaiseRequestProcessingEvent(PiccoloContext context)
		{
			var args = new RequestProcessingEvent {Context = context};
			RaiseEvent(_eventHandlers.RequestProcessing, args);
		}

		public void RaiseRequestProcessedEvent(PiccoloContext context)
		{
			var args = new RequestProcessedEvent {Context = context};
			RaiseEvent(_eventHandlers.RequestProcessed, args);
		}

		public void RaiseRequestFaultedEvent(PiccoloContext context, Exception exception)
		{
			var args = new RequestFaultedEvent {Context = context, Exception = exception};
			RaiseEvent(_eventHandlers.RequestFaulted, args);
		}

		private void RaiseEvent<TEvent>(IEnumerable<Type> eventHandlers, TEvent args) where TEvent : IEvent, new()
		{
			foreach (var eventHandlerType in eventHandlers)
			{
				var eventHandler = _objectFactory.CreateInstance<IHandle<TEvent>>(eventHandlerType);
				eventHandler.Handle(args);

				if (args.StopProcessing)
					break;
			}
		}
	}
}