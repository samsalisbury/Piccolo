using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CSharp;
using NUnit.Framework;
using Piccolo.Configuration;
using Piccolo.Events;
using Shouldly;

namespace Piccolo.Tests.Configuration
{
	public class EventHandlerScannerTests
	{
		[TestFixture]
		public class when_searching_for_handlers
		{
			private List<Type> _handlers;

			[SetUp]
			public void SetUp()
			{
				var @interface = typeof(IHandle<RequestProcessingEvent>);

				var method = new CodeMemberMethod();
				method.Attributes = MemberAttributes.Public;
				method.Name = "Handle";
				method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(RequestProcessingEvent), "args"));

				var lowPriorityHandler = new CodeTypeDeclaration("LowPriorityHandler");
				lowPriorityHandler.BaseTypes.Add(@interface);
				lowPriorityHandler.Members.Add(method);

				var highPriorityHandler = new CodeTypeDeclaration("HighPriorityHandler");
				highPriorityHandler.CustomAttributes.Add(new CodeAttributeDeclaration("Piccolo.Events.Priority", new CodeAttributeArgument(new CodePrimitiveExpression(1))));
				highPriorityHandler.BaseTypes.Add(@interface);
				highPriorityHandler.Members.Add(method);

				var @namespace = new CodeNamespace();
				@namespace.Types.Add(lowPriorityHandler);
				@namespace.Types.Add(highPriorityHandler);

				var assembly = new CodeCompileUnit();
				assembly.ReferencedAssemblies.Add("Piccolo.dll");
				assembly.Namespaces.Add(@namespace);

				var compilerParameters = new CompilerParameters
				{
					GenerateExecutable = false,
					IncludeDebugInformation = false,
					GenerateInMemory = true
				};

				var compiledAssembly = new CSharpCodeProvider().CompileAssemblyFromDom(compilerParameters, assembly).CompiledAssembly;
				_handlers = EventHandlerScanner.FindEventHandlersForEvent<RequestProcessingEvent>(compiledAssembly);
			}

			[Test]
			public void it_should_return_ordered_set_of_handlers()
			{
				_handlers.First().Name.ShouldBe("HighPriorityHandler");
			}
		}
	}
}