using System;

namespace Piccolo.Abstractions
{
	public interface IRequestContextWrapper
	{
		string Verb { get; }
		Uri Uri { get; }
	}
}