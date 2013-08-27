using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Piccolo.Events;
using Piccolo.Internal;
using Piccolo.Request.ParameterBinders;

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

		private static void DiscoverRequestHandlers(PiccoloConfiguration configuration, Assembly assembly)
		{
			var requestHandlers = assembly.GetExportedTypes().Where(x => x.GetInterfaces().Contains(typeof(IRequestHandler)));
			configuration.RequestHandlers = new ReadOnlyCollection<Type>(requestHandlers.ToList());
		}

		private static void DiscoverEventHandlers(PiccoloConfiguration configuration, Assembly assembly)
		{
			configuration.EventHandlers = new EventHandlers();
			configuration.EventHandlers.RequestProcessing = assembly.GetExportedTypes().Where(x => x.GetInterfaces().Contains(typeof(IHandle<RequestProcessingEvent>)));
		}

		private static void ApplyDefaultConfiguration(PiccoloConfiguration configuration)
		{
			configuration.RequestHandlerFactory = new DefaultRequestHandlerFactory();
			configuration.ParameterBinders = new Dictionary<Type, IParameterBinder>
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
			configuration.JsonSerialiser = model =>
			{
				var jsonSerializerSettings = new JsonSerializerSettings();
				jsonSerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

				return JsonConvert.SerializeObject(model, jsonSerializerSettings);
			};
			configuration.JsonDeserialiser = (type, payload) => JsonConvert.DeserializeObject(payload, type);
		}

		private static void RunStartupTasks(PiccoloConfiguration configuration, Assembly assembly)
		{
			var bootstrapperTypes = assembly.GetExportedTypes().Where(x => x.GetInterfaces().Contains(typeof(IStartupTask)));

			foreach (var bootstrapperType in bootstrapperTypes)
			{
				var instance = (IStartupTask)Activator.CreateInstance(bootstrapperType);
				instance.Run(configuration);
			}
		}

		private static bool AutomaticAssemblyDetectionFailed(Assembly assembly)
		{
			return assembly == typeof(HttpApplication).BaseType.Assembly;
		}
	}
}