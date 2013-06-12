using System.Net;
using System.Net.Http;

namespace Piccolo.Abstractions
{
	public class Response
	{
		public class Success
		{
			public static HttpResponseMessage Ok(string content)
			{
				return new HttpResponseMessage(HttpStatusCode.OK) {Content = new StringContent(content)};
			}
		}
	}
}