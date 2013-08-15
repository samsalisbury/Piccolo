# Piccolo
A modular micro-framework for creating APIs.

## Quickstart
**1)** New project (.net 4.0+)<br />
**2)** Run command `Install-Package Piccolo -Pre` in [Package Manager Console](http://docs.nuget.org/docs/start-here/using-the-package-manager-console) (you will need NuGet v2.6+)<br />
**3)** New class:<br />

    [Route("/")]
    public class Hello : IGet<string>
    {
    	public HttpResponseMessage<string> Get()
    	{
    		return Response.Success.Ok("Hello, Piccolo!");
    	}
    }

**4)** Build and run

## Documentation
- [Roadmap](https://github.com/opentable/Piccolo/wiki/Roadmap)

## License
MIT - see [license file](https://github.com/opentable/Piccolo/blob/master/LICENSE) for more information
