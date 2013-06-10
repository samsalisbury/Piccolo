using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Web;
using Piccolo.Configuration;

namespace Piccolo
{
	public class HttpHandler : IHttpHandler
	{
		[ExcludeFromCodeCoverage]
		public HttpHandler() : this(true)
		{
		}

		public HttpHandler(bool applyCustomConfiguration)
		{
			var bootstrapper = new Bootstrapper(Assembly.GetCallingAssembly());
			Configuration = bootstrapper.ApplyConfiguration(applyCustomConfiguration);
		}

		public void ProcessRequest(HttpContext context)
		{
			context.Response.Write("OK");
		}

		[ExcludeFromCodeCoverage]
		public bool IsReusable
		{
			get { return true; }
		}

		public HttpHandlerConfiguration Configuration { get; private set; }
	}
}