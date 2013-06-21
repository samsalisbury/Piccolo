using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using Piccolo.Abstractions;
using Piccolo.Configuration;

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
			var requestRouterResult = Configuration.Router.FindRequestHandler(requestContext.Verb, requestContext.Uri);
			if (requestRouterResult.HandlerType == null)
				return new HttpResponseMessage(HttpStatusCode.NotFound);

			var requestHandler = Configuration.RequestHandlerFactory.CreateInstance(requestRouterResult.HandlerType);

			// TODO: refactor the routing layer handles root routes (currently can't have more that one handler+verb combination)
			// TODO: implement handler property verification
			// TODO: refactor this mess
			if (requestContext.Verb.Equals("GET", StringComparison.InvariantCultureIgnoreCase))
			{
				// TODO: push properties
				return ((IGet<string>)requestHandler).Get().Message;
			}
			else if (requestContext.Verb.Equals("PUT", StringComparison.InvariantCultureIgnoreCase))
			{
				// TODO: push properties
				// TODO: pass post params
				return ((IPut<string>)requestHandler).Put("").Message;
			}
			else if (requestContext.Verb.Equals("POST", StringComparison.InvariantCultureIgnoreCase))
			{
				// TODO: push properties
				// TODO: pass post params
				return ((IPost<string>)requestHandler).Post("").Message;
			}
			else
			{
				return new HttpResponseMessage(HttpStatusCode.NotFound);
			}
		}
	}
}