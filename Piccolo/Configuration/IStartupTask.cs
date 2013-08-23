namespace Piccolo.Configuration
{
	public interface IStartupTask
	{
		void Run(PiccoloConfiguration configuration);
	}
}