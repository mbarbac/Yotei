﻿//using Microsoft.CSharp;
//using System.CodeDom.Compiler;
//using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;

namespace Experimental.Tests;

// ========================================================
//[Enforced]
//public static class Test_Runtime_Compiler
//{
//    //[Enforced]
//    [Fact]
//    public static void Test()
//    {
//        var provider = new CSharpCodeProvider();
//        var parameters = new CompilerParameters { GenerateInMemory = true };
//        var code = "public class RuntimeCode { public int Add(int a, int b) { return a + b; } }";
//        var results = provider.CompileAssemblyFromSource(parameters, code);
//        var instance = results.CompiledAssembly.CreateInstance("RuntimeCode");
//        var method = instance!.GetType().GetMethod("Add");
//        var result = (int)method!.Invoke(instance, [1, 2])!;
//        Assert.Equal(3, result);
//    }
//}