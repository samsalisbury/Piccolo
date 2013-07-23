using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using Piccolo.Configuration;
using Piccolo.Request;

namespace Piccolo
{
	public class HttpHandler : IHttpHandler
	{
		private readonly RequestHandlerInvoker _requestHandlerInvoker;

		[ExcludeFromCodeCoverage]
		public HttpHandler() : this(true, BuildManager.GetGlobalAsaxType().BaseType.Assembly)
		{
		}

		public HttpHandler(bool applyCustomConfiguration, Assembly assembly)
		{
			var bootstrapper = new Bootstrapper(assembly);
			Configuration = bootstrapper.ApplyConfiguration(applyCustomConfiguration);
			_requestHandlerInvoker = new RequestHandlerInvoker(Configuration.RouteParameterBinders);
		}

		public HttpHandlerConfiguration Configuration { get; private set; }

		[ExcludeFromCodeCoverage]
		public void ProcessRequest(HttpContext context)
		{
			// TODO: Implement response encoding
			var responseMessage = HandleRequest(new RequestContextWrapper(context));

			context.Response.StatusCode = (int)responseMessage.StatusCode;
			context.Response.StatusDescription = responseMessage.ReasonPhrase;
			if (responseMessage.Content != null)
				context.Response.Write(responseMessage.Content.ReadAsStringAsync().Result);
		}

		[ExcludeFromCodeCoverage]
		public bool IsReusable
		{
			get { return true; }
		}

		public HttpResponseMessage HandleRequest(IRequestContextWrapper requestContext)
		{
			var lookupResult = Configuration.Router.FindRequestHandler(requestContext.Verb, requestContext.Uri);
			if (lookupResult == null || lookupResult.RequestHandlerType == null)
				return new HttpResponseMessage(HttpStatusCode.NotFound);

			var queryParameters = HttpUtility.ParseQueryString(requestContext.Uri.Query).ToDictionary();

			var requestHandler = Configuration.RequestHandlerFactory.CreateInstance(lookupResult.RequestHandlerType);
			return _requestHandlerInvoker.Execute(requestHandler, requestContext.Verb, lookupResult.RouteParameters, queryParameters, requestContext.Payload);
		}
	}
}