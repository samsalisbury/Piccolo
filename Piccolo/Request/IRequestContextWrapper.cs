using System;

namespace Piccolo.Request
{
	public interface IRequestContextWrapper
	{
		string Verb { get; }
		Uri Uri { get; }
	}
}