#pragma warning disable IDE0290
#pragma warning disable CS0649

namespace Yotei.Tools.WithGenerator.Tests;

// ========================================================
//[Enforced]
public static partial class Test_RegularHost
{
    // EasyCase...
    partial class RType00
    {
        public RType00(RType00 _) { }
        [With] public string? Name { get; set; }
        [With] public int Age;
    }

    //[Enforced]
    [Fact]
    public static void Test_RType00()
    {
        MethodInfo method;
        ParameterInfo[] pars;
        var type = typeof(RType00);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(RType00), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType); // Verify '?' by visual inspection

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(RType00), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------
    // Easy case, UseVirtual...

    partial class RType01
    {
        public RType01(RType01 _) { }
        [With(UseVirtual = false)] public string? Name { get; set; }
        [With(UseVirtual = false)] public int Age;
    }

    //[Enforced]
    [Fact]
    public static void Test_RType01()
    {
        MethodInfo method;
        ParameterInfo[] pars;
        var type = typeof(RType01);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.False(method.IsVirtual);
        Assert.Equal(typeof(RType01), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType); // Verify '?' by visual inspection

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.False(method.IsVirtual);
        Assert.Equal(typeof(RType01), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Inherits from interface...
    partial interface IFace10 { [With] string? Name { get; } }
    [InheritsWith]
    partial class RType10 : IFace10
    {
        public RType10(RType10 _) { }
        public string? Name { get; set; }
        [With] public int Age;
    }
}