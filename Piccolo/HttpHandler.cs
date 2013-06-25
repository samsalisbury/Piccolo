using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
		[ExcludeFromCodeCoverage]
		public HttpHandler() : this(true, BuildManager.GetGlobalAsaxType().BaseType.Assembly)
		{
		}

		public HttpHandler(bool applyCustomConfiguration, Assembly assembly)
		{
			var bootstrapper = new Bootstrapper(assembly);
			Configuration = bootstrapper.ApplyConfiguration(applyCustomConfiguration);
		}

		public HttpHandlerConfiguration Configuration { get; private set; }

		[ExcludeFromCodeCoverage]
		public void ProcessRequest(HttpContext context)
		{
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

			var requestHandler = Configuration.RequestHandlerFactory.CreateInstance(lookupResult.RequestHandlerType);

			var requestHandlerInvoker = Configuration.RequestHandlerInvokers.Single(pair => pair.Key.Equals(requestContext.Verb, StringComparison.InvariantCultureIgnoreCase)).Value;
			return requestHandlerInvoker.Execute(requestHandler, lookupResult.RouteParameters);
		}
	}
}