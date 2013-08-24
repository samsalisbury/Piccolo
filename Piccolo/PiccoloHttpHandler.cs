using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using Piccolo.Configuration;
using Piccolo.Internal;
using Piccolo.Request;
using Piccolo.Routing;

namespace Piccolo
{
	public class PiccoloHttpHandler : IHttpHandler
	{
		private readonly PiccoloConfiguration _configuration;
		private readonly RequestHandlerInvoker _requestHandlerInvoker;
		private readonly RequestRouter _requestRouter;

		[ExcludeFromCodeCoverage]
		public PiccoloHttpHandler() : this(BuildManager.GetGlobalAsaxType().BaseType.Assembly, true)
		{
		}

		public PiccoloHttpHandler(Assembly assembly, bool applyCustomConfiguration)
		{
			_configuration = Bootstrapper.ApplyConfiguration(assembly, applyCustomConfiguration);
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
		    var lookupResult = _requestRouter.FindRequestHandler(context.RequestVerb, context.RequestUri);
		    if (lookupResult == null || lookupResult.RequestHandlerType == null)
		    {
		        InjectResponse(context, new HttpResponseMessage(HttpStatusCode.NotFound));
		        return;
		    }
		    
            var queryParameters = HttpUtility.ParseQueryString(context.RequestUri.Query).ToDictionary();

		    var requestHandler = _configuration.RequestHandlerFactory.CreateInstance(lookupResult.RequestHandlerType);
		    var httpResponseMessage = _requestHandlerInvoker.Execute(requestHandler, context.RequestVerb, lookupResult.RouteParameters, queryParameters, context.RequestPayload);
		    InjectResponse(context, httpResponseMessage);
		}

	    private void InjectResponse(PiccoloContext context, HttpResponseMessage responseMessage)
		{
			context.Http.Response.StatusCode = (int)responseMessage.StatusCode;
			context.Http.Response.StatusDescription = responseMessage.ReasonPhrase;

			if (responseMessage.HasContent())
			{
				context.Http.Response.AddHeader("Content-Type", "application/json");

				var objectContent = (ObjectContent)responseMessage.Content;
				var serialisedPayload = _configuration.JsonSerialiser(objectContent.Content);
				context.Http.Response.Write(serialisedPayload);
			}
		}
	}
}