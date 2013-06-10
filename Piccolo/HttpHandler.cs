using System.Web;

namespace Piccolo
{
	public class HttpHandler : IHttpHandler
	{
		public void ProcessRequest(HttpContext context)
		{
			context.Response.Write("OK");
		}

		public bool IsReusable
		{
			get { return true; }
		}
	}
}