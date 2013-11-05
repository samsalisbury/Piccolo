using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using Piccolo.Configuration;
using Piccolo.Events;
using Piccolo.Internal;
using Piccolo.Request;
using Piccolo.Routing;
using Piccolo.Validation;

namespace Piccolo
{
	public class PiccoloHttpHandler : IHttpHandler
	{
		private readonly PiccoloConfiguration _configuration;
		private readonly EventDispatcher _eventDispatcher;
		private readonly RequestHandlerInvoker _requestHandlerInvoker;
		private readonly RequestRouter _requestRouter;

		[ExcludeFromCodeCoverage]
		public PiccoloHttpHandler() : this(BuildManager.GetGlobalAsaxType().BaseType.Assembly, true)
		{
		}

		[ExcludeFromCodeCoverage]
		public PiccoloHttpHandler(Assembly assembly, bool applyCustomConfiguration) : this(Bootstrapper.ApplyConfiguration(assembly, applyCustomConfiguration))
		{
		}

		public PiccoloHttpHandler(PiccoloConfiguration configuration)
		{
			_configuration = configuration;
			_eventDispatcher = new EventDispatcher(_configuration.EventHandlers, _configuration.ObjectFactory);
			_requestRouter = new RequestRouter(_configuration.RequestHandlers);
			_requestHandlerInvoker = new RequestHandlerInvoker(_configuration.JsonDeserialiser, _configuration.ParameterBinders);
		}

		#region IHttpHandler

		[ExcludeFromCodeCoverage]
		public void ProcessRequest(HttpContext context)
		{
			ProcessRequest(new PiccoloContext(new HttpContextWrapper(context)));
		}

		[ExcludeFromCodeCoverage]
		public bool IsReusable
		{
			get { return true; }
		}

		#endregion

		public void ProcessRequest(PiccoloContext context)
		{
			string payload = null;
			try
			{
				var stopRequestProcessing = _eventDispatcher.RaiseRequestProcessingEvent(context);
				if (stopRequestProcessing)
					return;

				var lookupResult = _requestRouter.FindRequestHandler(context.RequestVerb, context.RequestUri);
				if (lookupResult.IsSuccessful)
				{
					var requestHandler = _configuration.ObjectFactory.CreateInstance<IRequestHandler>(lookupResult.RequestHandlerType);
					var payloadValidator = GetPayloadValidator(lookupResult.RequestHandlerType);
					var responseMessage = (HttpResponseMessage)_requestHandlerInvoker.Execute(requestHandler, context.RequestVerb, lookupResult.RouteParameters, context.RequestQueryParameters, context.Data, context.RequestPayload, payloadValidator);

					payload = SerialisePayload(responseMessage.Content);
					InjectResponse(context, responseMessage.StatusCode, responseMessage.ReasonPhrase, payload);
				}
				else
				{
					var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
					InjectResponse(context, responseMessage.StatusCode, responseMessage.ReasonPhrase, null);
				}
			}
			catch (RouteParameterDatatypeMismatchException rpdmex)
			{
				_eventDispatcher.RaiseRequestFaultedEvent(context, rpdmex);

				var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
				if (context.Http.IsDebuggingEnabled)
					payload = SerialisePayload(new ObjectContent(rpdmex));

				InjectResponse(context, responseMessage.StatusCode, responseMessage.ReasonPhrase, payload);
			}
			catch (MalformedParameterException mpex)
			{
				_eventDispatcher.RaiseRequestFaultedEvent(context, mpex);

				var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
				if (context.Http.IsDebuggingEnabled)
					payload = SerialisePayload(new ObjectContent(mpex));

				InjectResponse(context, responseMessage.StatusCode, responseMessage.ReasonPhrase, payload);
			}
			catch (MissingPayloadException mpex)
			{
				_eventDispatcher.RaiseRequestFaultedEvent(context, mpex);

				var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
				payload = SerialisePayload(new ObjectContent(new {error = "Payload missing"}));

				InjectResponse(context, responseMessage.StatusCode, responseMessage.ReasonPhrase, payload);
			}
			catch (MalformedPayloadException mpex)
			{
				_eventDispatcher.RaiseRequestFaultedEvent(context, mpex);

				if (context.Http.IsDebuggingEnabled)
					payload = SerialisePayload(new ObjectContent(mpex));

				InjectResponse(context, (HttpStatusCode)422, "Unprocessable Entity", payload);
			}
			catch (Exception ex)
			{
				_eventDispatcher.RaiseRequestFaultedEvent(context, ex);

				var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
				if (context.Http.IsDebuggingEnabled)
					payload = SerialisePayload(new ObjectContent(ex));

				InjectResponse(context, responseMessage.StatusCode, responseMessage.ReasonPhrase, payload);
			}
			finally
			{
				_eventDispatcher.RaiseRequestProcessedEvent(context, payload);
			}
		}

		private object GetPayloadValidator(Type requestHandlerType)
		{
			var attribute = requestHandlerType.GetCustomAttributes(typeof(ValidateWithAttribute), true).Cast<ValidateWithAttribute>().SingleOrDefault();
			return attribute == null ? null : _configuration.ObjectFactory.CreateInstance<object>(attribute.ValidatorType);
		}

		private string SerialisePayload(HttpContent httpContent)
		{
			if (httpContent == null)
				return null;

			var objectContent = (ObjectContent)httpContent;
			return _configuration.JsonSerialiser(objectContent.Content);
		}

		private static void InjectResponse(PiccoloContext context, HttpStatusCode statusCode, string reasonPhrase, string payload)
		{
			context.Http.Response.StatusCode = (int)statusCode;
			context.Http.Response.StatusDescription = reasonPhrase;

			if (payload != null)
			{
				context.Http.Response.ContentType = "application/json";
				context.Http.Response.Write(payload);
			}
		}
	}
}