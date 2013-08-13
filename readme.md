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

## Roadmap
*IN-PROGRESS*

*MOVE TO WIKI*

*LINK TO WIKI*

- [x] Core HTTP verbs: GET, POST, PUT, DELETE
- [x] Static routing
- [x] Dynamic routing
- [x] Samples
- [ ] Validation
- [ ] CORS
- [x] IoC
- [ ] Nullable optional parameters in request handlers
- [x] JSON encoding/decoding
- [ ] NuGet package
- [ ] Public nightly build
- [ ] Awesome documentation!
