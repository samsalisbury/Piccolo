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

		public bool RaiseRequestProcessingEvent(PiccoloContext context)
		{
			var args = new RequestProcessingEvent {Context = context};
			var stopRequestProcessing = false;

			foreach (var eventHandlerType in _eventHandlers.RequestProcessing)
			{
				var eventHandler = _objectFactory.CreateInstance<IHandle<RequestProcessingEvent>>(eventHandlerType);
				eventHandler.Handle(args);

				stopRequestProcessing = args.StopRequestProcessing;

				if (args.StopEventProcessing)
					break;
			}

			return stopRequestProcessing;
		}

		public void RaiseRequestProcessedEvent(PiccoloContext context, string payload)
		{
			var args = new RequestProcessedEvent {Context = context, Payload = payload};
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

				if (args.StopEventProcessing)
					break;
			}
		}
	}
}