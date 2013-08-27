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
			RaiseEvent<RequestProcessingEvent>(_eventHandlers.RequestProcessing, context);
		}

		public void RaiseRequestProcessedEvent(PiccoloContext context)
		{
			RaiseEvent<RequestProcessedEvent>(_eventHandlers.RequestProcessed, context);
		}

		private void RaiseEvent<TEvent>(IEnumerable<Type> eventHandlers, PiccoloContext context) where TEvent : IEvent, new()
		{
			var args = new TEvent {Context = context};

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