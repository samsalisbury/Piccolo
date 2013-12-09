using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;
using Microsoft.CSharp;

namespace Piccolo.Tests
{
	internal static class RuntimeAssemblyBuilder
	{
		internal static Assembly BuildAssembly(params CodeTypeDeclaration[] typeDeclarations)
		{
			var @namespace = new CodeNamespace();
			@namespace.Types.AddRange(typeDeclarations);

			var assembly = new CodeCompileUnit();
			assembly.ReferencedAssemblies.Add("Piccolo.dll");
			assembly.Namespaces.Add(@namespace);

			var compilerParameters = new CompilerParameters
			{
				GenerateExecutable = false,
				IncludeDebugInformation = false,
				GenerateInMemory = true
			};

			CompilerResults compileAssemblyFromDom = new CSharpCodeProvider().CompileAssemblyFromDom(compilerParameters, assembly);
			var err = compileAssemblyFromDom.Errors;
			return compileAssemblyFromDom.CompiledAssembly;
		}
	}
}