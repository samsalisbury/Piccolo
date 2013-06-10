namespace Piccolo.Configuration
{
	public interface IStartupTask
	{
		void Run(HttpHandlerConfiguration configuration);
	}
}