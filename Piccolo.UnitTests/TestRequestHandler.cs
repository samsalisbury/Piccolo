﻿using System.Diagnostics.CodeAnalysis;
using System.Net.Http;

namespace Piccolo.UnitTests
{
	[Route("/data/resources/{id}")]
	public class TestRequestHandler : IGet<string>
	{
		[ExcludeFromCodeCoverage]
		public HttpResponseMessage<string> Get()
		{
			return new HttpResponseMessage<string>();
		}

		public int Id { get; set; }
	}
}