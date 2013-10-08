using System;
using System.Diagnostics.CodeAnalysis;
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
			try
			{
				var stopRequestProcessing = _eventDispatcher.RaiseRequestProcessingEvent(context);
				if (stopRequestProcessing)
					return;

				var lookupResult = _requestRouter.FindRequestHandler(context.RequestVerb, context.RequestUri);
				if (lookupResult.IsSuccessful)
				{
					var requestHandler = _configuration.ObjectFactory.CreateInstance<IRequestHandler>(lookupResult.RequestHandlerType);
					var httpResponseMessage = _requestHandlerInvoker.Execute(requestHandler, context.RequestVerb, lookupResult.RouteParameters, context.RequestQueryParameters, context.Data, context.RequestPayload, null);
					InjectResponse(context, httpResponseMessage);
				}
				else
					InjectResponse(context, new HttpResponseMessage(HttpStatusCode.NotFound));
			}
			catch (RouteParameterDatatypeMismatchException rpdmex)
			{
				_eventDispatcher.RaiseRequestFaultedEvent(context, rpdmex);

				var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
				if (context.Http.IsDebuggingEnabled)
					httpResponseMessage.Content = new ObjectContent(rpdmex);

				InjectResponse(context, httpResponseMessage);
			}
			catch (MalformedParameterException mpex)
			{
				_eventDispatcher.RaiseRequestFaultedEvent(context, mpex);

				var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
				if (context.Http.IsDebuggingEnabled)
					httpResponseMessage.Content = new ObjectContent(mpex);

				InjectResponse(context, httpResponseMessage);
			}
			catch (MalformedPayloadException mpex)
			{
				_eventDispatcher.RaiseRequestFaultedEvent(context, mpex);

				var httpResponseMessage = new HttpResponseMessage
				{
					StatusCode = (HttpStatusCode)422,
					ReasonPhrase = "Unprocessable Entity",
					Content = context.Http.IsDebuggingEnabled ? new ObjectContent(mpex) : null
				};

				InjectResponse(context, httpResponseMessage);
			}
			catch (Exception ex)
			{
				_eventDispatcher.RaiseRequestFaultedEvent(context, ex);

				var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
				if (context.Http.IsDebuggingEnabled)
					httpResponseMessage.Content = new ObjectContent(ex);

				InjectResponse(context, httpResponseMessage);
			}
			finally
			{
				_eventDispatcher.RaiseRequestProcessedEvent(context);
			}
		}

		private void InjectResponse(PiccoloContext context, HttpResponseMessage responseMessage)
		{
			context.Http.Response.StatusCode = (int)responseMessage.StatusCode;
			context.Http.Response.StatusDescription = responseMessage.ReasonPhrase;

			if (responseMessage.HasContent())
			{
				context.Http.Response.ContentType = "application/json";
				context.Http.Response.Write(SerialisePayload(responseMessage));
			}
		}

		private string SerialisePayload(HttpResponseMessage responseMessage)
		{
			var objectContent = (ObjectContent)responseMessage.Content;
			return _configuration.JsonSerialiser(objectContent.Content);
		}
	}
}