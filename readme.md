# Piccolo
A modular micro-framework for creating APIs.

## Quickstart
**1)** New web application project (.net 4.0+)<br />
**2)** Run command `Install-Package Piccolo -Pre` in [Package Manager Console](http://docs.nuget.org/docs/start-here/using-the-package-manager-console) (you will need NuGet v2.6+)<br />
**3)** New class:<br />

```csharp
[Route("/")]
public class Hello : IGet<string>
{
	public HttpResponseMessage<string> Get()
	{
		return Response.Success.Ok("Hello, Piccolo!");
	}
}
```

**4)** Build and run

## Documentation
1. [Installation](https://github.com/opentable/Piccolo/wiki/Installation)
1. [GET Request Handlers](https://github.com/opentable/Piccolo/wiki/GET-Request-Handlers)
1. [POST Request Handlers](https://github.com/opentable/Piccolo/wiki/POST-Request-Handlers)
1. [PUT Request Handlers](https://github.com/opentable/Piccolo/wiki/PUT-Request-Handlers)
1. [DELETE Request Handlers](https://github.com/opentable/Piccolo/wiki/DELETE-Request-Handlers)
1. [Routing](https://github.com/opentable/Piccolo/wiki/Routing)
1. [Response Helpers](https://github.com/opentable/Piccolo/wiki/Response-Helpers)
1. [JSON Serialisation](https://github.com/opentable/Piccolo/wiki/JSON-Serialisation)
1. [JSON Deserialisation](https://github.com/opentable/Piccolo/wiki/JSON-Deserialisation)
1. [Configuration](https://github.com/opentable/Piccolo/wiki/Configuration)
1. Advanced Concepts
    1. [Start-up Tasks](https://github.com/opentable/Piccolo/wiki/Startup-Tasks)
    1. [Implementing Custom Request Handler Factory](https://github.com/opentable/Piccolo/wiki/Implementing-Custom-Request-Handler-Factory)
    1. [Implementing Custom Parameter Binders](https://github.com/opentable/Piccolo/wiki/Implementing-Custom-Parameter-Binders)
    1. [Overriding JSON Serialisation](https://github.com/opentable/Piccolo/wiki/Overriding-JSON-Serialisation)
    1. [Overriding JSON Deserialisation](https://github.com/opentable/Piccolo/wiki/Overriding-JSON-Deserialisation)
    1. [High-Level Architecture](https://github.com/opentable/Piccolo/wiki/High-Level-Architecture)
1. [Roadmap](https://github.com/opentable/Piccolo/wiki/Roadmap)

## License
MIT - see [license file](https://github.com/opentable/Piccolo/blob/master/LICENSE) for more information
