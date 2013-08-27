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

		public PiccoloHttpHandler(Assembly assembly, bool applyCustomConfiguration)
		{
			_configuration = Bootstrapper.ApplyConfiguration(assembly, applyCustomConfiguration);
			_eventDispatcher = new EventDispatcher(_configuration.EventHandlers);
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
				_eventDispatcher.RaiseRequestProcessingEvent(context);

				var lookupResult = _requestRouter.FindRequestHandler(context.RequestVerb, context.RequestUri);
				if (lookupResult.IsSuccessful)
				{
					var requestHandler = _configuration.ObjectFactory.CreateInstance(lookupResult.RequestHandlerType);
					var httpResponseMessage = _requestHandlerInvoker.Execute(requestHandler, context.RequestVerb, lookupResult.RouteParameters, context.RequestQueryParameters, context.RequestPayload);
					InjectResponse(context, httpResponseMessage);
				}
				else
					InjectResponse(context, new HttpResponseMessage(HttpStatusCode.NotFound));
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