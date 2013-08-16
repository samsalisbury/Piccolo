using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using Piccolo.Configuration;
using Piccolo.Internal;
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
			EnsureValidAssembly(assembly);

			var bootstrapper = new Bootstrapper(assembly);
			Configuration = bootstrapper.ApplyConfiguration(applyCustomConfiguration);
			_requestHandlerInvoker = new RequestHandlerInvoker(Configuration.JsonDeserialiser, Configuration.RouteParameterBinders);
		}

		public HttpHandlerConfiguration Configuration { get; private set; }

		[ExcludeFromCodeCoverage]
		public void ProcessRequest(HttpContext context)
		{
			ProcessRequest(new HttpContextWrapper(context));
		}

		[ExcludeFromCodeCoverage]
		public bool IsReusable
		{
			get { return true; }
		}

		public void ProcessRequest(HttpContextBase httpContext)
		{
			var verb = httpContext.Request.HttpMethod;
			var requestUri = httpContext.Request.Url;
			var payload = GetPayload(httpContext);
			var responseMessage = HandleRequest(verb, requestUri, payload);

			httpContext.Response.StatusCode = (int)responseMessage.StatusCode;
			httpContext.Response.StatusDescription = responseMessage.ReasonPhrase;

			if (responseMessage.Content != null)
			{
				var objectContent = (ObjectContent)responseMessage.Content;
				var serialisedPayload = Configuration.JsonSerialiser(objectContent.Content);
				httpContext.Response.Write(serialisedPayload);
			}
		}

		private HttpResponseMessage HandleRequest(string verb, Uri requestUri, string payload)
		{
			var lookupResult = Configuration.Router.FindRequestHandler(verb, requestUri);
			if (lookupResult == null || lookupResult.RequestHandlerType == null)
				return new HttpResponseMessage(HttpStatusCode.NotFound);

			var queryParameters = HttpUtility.ParseQueryString(requestUri.Query).ToDictionary();

			var requestHandler = Configuration.RequestHandlerFactory.CreateInstance(lookupResult.RequestHandlerType);
			return _requestHandlerInvoker.Execute(requestHandler, verb, lookupResult.RouteParameters, queryParameters, payload);
		}

		private static string GetPayload(HttpContextBase httpContext)
		{
			if (httpContext.Request.InputStream.CanRead == false)
				return string.Empty;

			using (var reader = new StreamReader(httpContext.Request.InputStream))
				return reader.ReadToEnd();
		}

		private static void EnsureValidAssembly(Assembly assembly)
		{
			if (assembly == typeof(HttpApplication).Assembly)
				throw new InvalidOperationException(ExceptionMessageBuilder.BuildMissingGlobalAsaxMessage());
		}
	}
}