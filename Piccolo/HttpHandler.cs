using System.Diagnostics.CodeAnalysis;
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

			context.Response.Write(responseMessage.Content.ReadAsStringAsync().Result);
		}

		[ExcludeFromCodeCoverage]
		public bool IsReusable
		{
			get { return true; }
		}

		public HttpResponseMessage HandleRequest(IRequestContextWrapper requestContext)
		{
			var requestHandlerType = Configuration.Router.GetRequestHandlerForUri(requestContext.Uri);
			// TODO: handle null type

			var requestHandler = Configuration.RequestHandlerFactory.CreateInstance(requestHandlerType);
			// TODO: push properties

			// TODO: handle other verbs
			// TODO: handle other return types

			return ((IGet<string>)requestHandler).Get();
		}
	}
}