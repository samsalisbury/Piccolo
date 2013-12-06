using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using Piccolo.Configuration;
using Piccolo.Events;
using Piccolo.Request;
using Piccolo.Routing;

namespace Piccolo
{
	public class PiccoloHttpHandler : IHttpHandler
	{
		public PiccoloHttpHandler() : this(BuildManager.GetGlobalAsaxType().BaseType.Assembly)
		{
			/* This requires a Global.asax (even an empty one) to be place in the root directory of the application.
			 * NB: Could't find a cleaer way to auto-detecting this. None of the usual Assembly.Get***Assembly() methods return what I need. */
		}

		public PiccoloHttpHandler(Assembly assembly)
		{
			Configuration = Bootstrapper.ApplyConfiguration(assembly, true);

			var eventDispatcher = new EventDispatcher(Configuration.EventHandlers, Configuration.ObjectFactory);
			var requestRouter = new RequestRouter(Configuration.RequestHandlers);
			var requestHandlerInvoker = new RequestHandlerInvoker(Configuration.JsonDeserialiser, Configuration.ParameterBinders);

			Engine = new PiccoloEngine(Configuration, eventDispatcher, requestRouter, requestHandlerInvoker);
		}

		public PiccoloConfiguration Configuration { get; private set; }
		public PiccoloEngine Engine { get; private set; }

		#region IHttpHandler

		[ExcludeFromCodeCoverage]
		public void ProcessRequest(HttpContext context)
		{
			Engine.ProcessRequest(new PiccoloContext(new HttpContextWrapper(context)));
		}

		public bool IsReusable
		{
			get { return true; }
		}

		#endregion
	}
}