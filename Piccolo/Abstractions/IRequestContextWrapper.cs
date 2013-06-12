using System;

namespace Piccolo.Abstractions
{
	public interface IRequestContextWrapper
	{
		Uri Uri { get; }
	}
}