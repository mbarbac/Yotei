using Microsoft.CSharp;
using System.CodeDom.Compiler;

namespace Experiments.LangFeatures;

// ========================================================
[Enforced]
public static class Test_RuntimeCompiler
{
    //[Enforced]
    //[Fact]
    //public static void Test_()
    //{
    //    var provider = new CSharpCodeProvider();
    //    var parameters = new CompilerParameters { GenerateInMemory = true };
    //    var code = "public class RuntimeCode { public int Add(int a, int b) { return a + b; } }";
    //    var results = provider.CompileAssemblyFromSource(parameters, code);
    //    var instance = results.CompiledAssembly.CreateInstance("RuntimeCode");
    //    var method = instance!.GetType().GetMethod("Add");
    //    var result = (int)method!.Invoke(instance, [1, 2])!;
    //    Assert.Equal(3, result);
    //}
}