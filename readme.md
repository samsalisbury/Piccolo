# Piccolo

A modular micro-framework for creating APIs.

## Quickstart
1. File > New Project (.NET 4.0+)
2. `Install-Package Piccolo -Pre`
3. Create handler:

		[Route("/")]
    	public class Hello : IGet<string>
    	{
	    	public HttpResponseMessage<string> Get()
    		{
    			return Response.Success.Ok("Hello, Piccolo!");
    		}
    	}
4. Build and run
