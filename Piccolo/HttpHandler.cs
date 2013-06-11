using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Web;
using Piccolo.Configuration;

namespace Piccolo
{
	public class HttpHandler : IHttpHandler
	{
		public HttpHandler() : this(true, Assembly.GetCallingAssembly())
		{
		}

		public HttpHandler(bool applyCustomConfiguration, Assembly assembly)
		{
			var bootstrapper = new Bootstrapper(assembly);
			Configuration = bootstrapper.ApplyConfiguration(applyCustomConfiguration);
		}

		public HttpHandlerConfiguration Configuration { get; private set; }

		public void ProcessRequest(HttpContext context)
		{
			context.Response.Write("OK");
		}

		[ExcludeFromCodeCoverage]
		public bool IsReusable
		{
			get { return true; }
		}
	}
}