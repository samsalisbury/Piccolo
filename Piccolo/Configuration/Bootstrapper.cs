using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Piccolo.Request.ParameterBinders;
using Piccolo.Routing;

namespace Piccolo.Configuration
{
	public class Bootstrapper
	{
		private readonly Assembly _assembly;

		public Bootstrapper(Assembly assembly)
		{
			_assembly = assembly;
		}

		public HttpHandlerConfiguration ApplyConfiguration(bool applyCustomConfiguration)
		{
			var configuration = new HttpHandlerConfiguration();

			DiscoverRequestHandlers(configuration, _assembly);
			ApplyDefaultConfiguration(configuration);

			if (applyCustomConfiguration)
				RunStartupTasks(configuration, _assembly);

			return configuration;
		}

		private static void DiscoverRequestHandlers(HttpHandlerConfiguration configuration, Assembly assembly)
		{
			var requestHandlers = assembly.GetExportedTypes().Where(x => x.GetInterfaces().Contains(typeof(IRequestHandler)));
			configuration.RequestHandlers = new ReadOnlyCollection<Type>(requestHandlers.ToList());
		}

		private static void ApplyDefaultConfiguration(HttpHandlerConfiguration configuration)
		{
			configuration.RequestHandlerFactory = new DefaultRequestHandlerFactory();
			configuration.Router = new RequestRouter(configuration.RequestHandlers);
			configuration.RouteParameterBinders = new Dictionary<Type, IParameterBinder>
			{
				{typeof(String), new StringParameterBinder()},
				{typeof(Boolean), new BooleanParameterBinder()},
				{typeof(Boolean?), new NullableBooleanParameterBinder()},
				{typeof(Byte), new ByteParameterBinder()},
				{typeof(Byte?), new NullableByteParameterBinder()},
				{typeof(Int16), new Int16ParameterBinder()},
				{typeof(Int16?), new NullableInt16ParameterBinder()},
				{typeof(Int32), new Int32ParameterBinder()},
				{typeof(Int32?), new NullableInt32ParameterBinder()},
				{typeof(DateTime), new DateTimeParameterBinder()},
				{typeof(DateTime?), new NullableDateTimeParameterBinder()}
			};
			configuration.JsonSerialiser = JsonConvert.SerializeObject;
			configuration.JsonDeserialiser = (type, payload) => JsonConvert.DeserializeObject(payload, type);
		}

		private static void RunStartupTasks(HttpHandlerConfiguration configuration, Assembly assembly)
		{
			var bootstrapperTypes = assembly.GetExportedTypes().Where(x => x.GetInterfaces().Contains(typeof(IStartupTask)));

			foreach (var bootstrapperType in bootstrapperTypes)
			{
				var instance = (IStartupTask)Activator.CreateInstance(bootstrapperType);
				instance.Run(configuration);
			}
		}
	}
}