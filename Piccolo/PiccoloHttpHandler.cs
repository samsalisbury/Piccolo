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
using Piccolo.Routing;

namespace Piccolo
{
	public class PiccoloHttpHandler : IHttpHandler
	{
		private readonly PiccoloConfiguration _configuration;
		private readonly RequestRouter _requestRouter;
		private readonly RequestHandlerInvoker _requestHandlerInvoker;

		[ExcludeFromCodeCoverage]
		public PiccoloHttpHandler() : this(BuildManager.GetGlobalAsaxType().BaseType.Assembly, true)
		{
		}

		public PiccoloHttpHandler(Assembly assembly, bool applyCustomConfiguration)
		{
			EnsureValidAssembly(assembly);

			_configuration = new Bootstrapper().ApplyConfiguration(assembly, applyCustomConfiguration);
			_requestRouter = new RequestRouter(_configuration.RequestHandlers);
			_requestHandlerInvoker = new RequestHandlerInvoker(_configuration.JsonDeserialiser, _configuration.ParameterBinders);
		}

		#region IHttpHandler

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

		#endregion

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
				httpContext.Response.AddHeader("Content-Type", "application/json");

				var objectContent = (ObjectContent)responseMessage.Content;
				var serialisedPayload = _configuration.JsonSerialiser(objectContent.Content);
				httpContext.Response.Write(serialisedPayload);
			}
		}

		private HttpResponseMessage HandleRequest(string verb, Uri requestUri, string payload)
		{
			var lookupResult = _requestRouter.FindRequestHandler(verb, requestUri);
			if (lookupResult == null || lookupResult.RequestHandlerType == null)
				return new HttpResponseMessage(HttpStatusCode.NotFound);

			var queryParameters = HttpUtility.ParseQueryString(requestUri.Query).ToDictionary();

			var requestHandler = _configuration.RequestHandlerFactory.CreateInstance(lookupResult.RequestHandlerType);
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
			if (assembly == typeof(HttpApplication).BaseType.Assembly)
				throw new InvalidOperationException(ExceptionMessageBuilder.BuildMissingGlobalAsaxMessage());
		}
	}
}