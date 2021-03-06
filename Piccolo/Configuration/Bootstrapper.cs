using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Piccolo.Events;
using Piccolo.Internal;

namespace Piccolo.Configuration
{
	public static class Bootstrapper
	{
		public static PiccoloConfiguration ApplyConfiguration(Assembly assembly, bool applyCustomConfiguration)
		{
			if (AutomaticAssemblyDetectionFailed(assembly))
				throw new InvalidOperationException(ExceptionMessageBuilder.BuildMissingGlobalAsaxMessage());

			var configuration = new PiccoloConfiguration();

			DiscoverRequestHandlers(configuration, assembly);
			DiscoverEventHandlers(configuration, assembly);
			ApplyDefaultConfiguration(configuration);

			if (applyCustomConfiguration)
				RunStartupTasks(configuration, assembly);

			return configuration;
		}

		private static bool AutomaticAssemblyDetectionFailed(Assembly assembly)
		{
			return assembly == typeof(HttpApplication).BaseType.Assembly;
		}

		private static void DiscoverRequestHandlers(PiccoloConfiguration configuration, Assembly assembly)
		{
			var requestHandlers = assembly.GetTypesImplementing<IRequestHandler>();
			configuration.RequestHandlers = new ReadOnlyCollection<Type>(requestHandlers);
		}

		private static void DiscoverEventHandlers(PiccoloConfiguration configuration, Assembly assembly)
		{
			var requestProcessingHandlers = EventHandlerScanner.FindEventHandlersForEvent<RequestProcessingEvent>(assembly);
			var requestFaultedHandlers = EventHandlerScanner.FindEventHandlersForEvent<RequestFaultedEvent>(assembly);
			var requestProcessedHandlers = EventHandlerScanner.FindEventHandlersForEvent<RequestProcessedEvent>(assembly);

			configuration.EventHandlers = new EventHandlers(requestProcessingHandlers, requestFaultedHandlers, requestProcessedHandlers);
		}

		private static void ApplyDefaultConfiguration(PiccoloConfiguration configuration)
		{
			configuration.ObjectFactory = new DefaultObjectFactory();
			configuration.Parsers = new Dictionary<Type, Func<string, object>>
			{
				{typeof(String), x => x},
				{typeof(Boolean), x => Boolean.Parse(x)},
				{typeof(Boolean?), x => Boolean.Parse(x)},
				{typeof(Int32), x => Int32.Parse(x)},
				{typeof(Int32?), x => Int32.Parse(x)},
				{typeof(DateTime), x => DateTime.Parse(x)},
				{typeof(DateTime?), x => DateTime.Parse(x)}
			};
			configuration.JsonSerialiser = model =>
			{
				var jsonSerializerSettings = new JsonSerializerSettings
				{
					ContractResolver = new CamelCasePropertyNamesContractResolver()
				};

				return JsonConvert.SerializeObject(model, jsonSerializerSettings);
			};
			configuration.JsonDeserialiser = (type, payload) => JsonConvert.DeserializeObject(payload, type);
		}

		private static void RunStartupTasks(PiccoloConfiguration configuration, Assembly assembly)
		{
			var bootstrapperTypes = assembly.GetTypesImplementing<IStartupTask>();

			foreach (var bootstrapperType in bootstrapperTypes)
			{
				var instance = (IStartupTask)Activator.CreateInstance(bootstrapperType);
				instance.Run(configuration);
			}
		}
	}
}