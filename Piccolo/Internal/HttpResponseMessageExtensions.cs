using System.Net.Http;

namespace Piccolo.Internal
{
	internal static class HttpResponseMessageExtensions
	{
		public static bool HasContent(this HttpResponseMessage httpResponseMessage)
		{
			return httpResponseMessage.Content != null;
		}
	}
}