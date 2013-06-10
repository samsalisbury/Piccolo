using Piccolo.IoC;

namespace Piccolo.Configuration
{
	public class HttpHandlerConfiguration
	{
		internal HttpHandlerConfiguration()
		{
		}

		public IRequestHandlerFactory RequestHandlerFactory { get; set; }
	}
}