using System;
using System.Net;
using System.Net.Http;
using Piccolo.Configuration;
using Piccolo.Events;
using Piccolo.Request;
using Piccolo.Routing;
using Piccolo.Validation;

namespace Piccolo
{
	public class PiccoloEngine
	{
		private readonly PiccoloConfiguration _configuration;
		private readonly IEventDispatcher _eventDispatcher;
		private readonly IPayloadDeserialiser _payloadDeserialiser;
		private readonly IPayloadValidatorInvoker _payloadValidatorInvoker;
		private readonly IRequestHandlerInvoker _requestHandlerInvoker;
		private readonly IRequestRouter _requestRouter;

		public PiccoloEngine(PiccoloConfiguration configuration, IEventDispatcher eventDispatcher, IRequestRouter requestRouter, IRequestHandlerInvoker requestHandlerInvoker)
		{
			_configuration = configuration;
			_eventDispatcher = eventDispatcher;
			_requestRouter = requestRouter;
			_payloadDeserialiser = new PayloadDeserialiser(configuration.JsonDeserialiser);
			_payloadValidatorInvoker = new PayloadValidatorInvoker(configuration.ObjectFactory);
			_requestHandlerInvoker = requestHandlerInvoker;
		}

		public void ProcessRequest(PiccoloContext context)
		{
			string payload = null;
			try
			{
				var stopRequestProcessing = _eventDispatcher.RaiseRequestProcessingEvent(context);
				if (stopRequestProcessing)
					return;

				var lookupResult = _requestRouter.FindRequestHandler(context.RequestVerb, context.ApplicationPath, context.RequestUri);
				if (lookupResult.IsSuccessful)
				{
					var requestHandler = _configuration.ObjectFactory.CreateInstance<IRequestHandler>(lookupResult.RequestHandlerType);
					var requestPayload = _payloadDeserialiser.DeserialisePayload(requestHandler, context.RequestVerb, context.RequestPayload);
					if (requestPayload != null)
					{
						string errorMessage;
						if ((errorMessage = _payloadValidatorInvoker.ValidatePayload(lookupResult.RequestHandlerType, context.RequestVerb, requestPayload)) != null)
						{
							InjectResponse(context, HttpStatusCode.BadRequest, "Bad Request", null, errorMessage);
							return;
						}
					}

					var responseMessage = _requestHandlerInvoker.Execute(requestHandler, context.RequestVerb, lookupResult.RouteParameters, context.RequestQueryParameters, context.Data, requestPayload);

					payload = SerialisePayload(responseMessage.Content);
					InjectResponse(context, responseMessage.StatusCode, responseMessage.ReasonPhrase, responseMessage.Headers.Location, payload);
				}
				else
				{
					var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
					InjectResponse(context, responseMessage.StatusCode, responseMessage.ReasonPhrase, null, null);
				}
			}
			catch (RouteParameterDatatypeMismatchException rpdmex)
			{
				_eventDispatcher.RaiseRequestFaultedEvent(context, rpdmex);

				var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
				if (context.Http.IsDebuggingEnabled)
					payload = SerialisePayload(new ObjectContent(rpdmex));

				InjectResponse(context, responseMessage.StatusCode, responseMessage.ReasonPhrase, null, payload);
			}
			catch (MalformedParameterException mpex)
			{
				_eventDispatcher.RaiseRequestFaultedEvent(context, mpex);

				var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
				if (context.Http.IsDebuggingEnabled)
					payload = SerialisePayload(new ObjectContent(mpex));

				InjectResponse(context, responseMessage.StatusCode, responseMessage.ReasonPhrase, null, payload);
			}
			catch (MissingPayloadException mpex)
			{
				_eventDispatcher.RaiseRequestFaultedEvent(context, mpex);

				var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
				payload = SerialisePayload(new ObjectContent(new {error = "Payload missing"}));

				InjectResponse(context, responseMessage.StatusCode, responseMessage.ReasonPhrase, null, payload);
			}
			catch (MalformedPayloadException mpex)
			{
				_eventDispatcher.RaiseRequestFaultedEvent(context, mpex);

				if (context.Http.IsDebuggingEnabled)
					payload = SerialisePayload(new ObjectContent(mpex));

				InjectResponse(context, (HttpStatusCode)422, "Unprocessable Entity", null, payload);
			}
			catch (Exception ex)
			{
				_eventDispatcher.RaiseRequestFaultedEvent(context, ex);

				var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
				if (context.Http.IsDebuggingEnabled)
					payload = SerialisePayload(new ObjectContent(ex));

				InjectResponse(context, responseMessage.StatusCode, responseMessage.ReasonPhrase, null, payload);
			}
			finally
			{
				_eventDispatcher.RaiseRequestProcessedEvent(context, payload);
			}
		}

		private string SerialisePayload(HttpContent httpContent)
		{
			if (httpContent == null)
				return null;

			var objectContent = (ObjectContent)httpContent;
			return _configuration.JsonSerialiser(objectContent.Content);
		}

		private static void InjectResponse(PiccoloContext context, HttpStatusCode statusCode, string reasonPhrase, Uri createdResourceLocation, string payload)
		{
			context.Http.Response.StatusCode = (int)statusCode;
			context.Http.Response.StatusDescription = reasonPhrase;

			if (createdResourceLocation != null)
				context.Http.Response.AddHeader("Location", createdResourceLocation.ToString());

			if (payload != null)
			{
				context.Http.Response.ContentType = "application/json";
				context.Http.Response.Write(payload);
			}
		}
	}
}