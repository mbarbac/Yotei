namespace Yotei.Tools.WithGenerator.Tests;

// ========================================================
//[Enforced]
public partial class Test_RegularHost
{
    // Default case...
    partial class RType01A
    {
        public RType01A(string name) => Name = name;
        protected RType01A(RType01A source) => Name = source.Name;
        [With] public string? Name { get; init; }
        [With] public int Age = 0;
    }

    //[Enforced]
    [Fact]
    public static void Test_Type01A()
    {
        MethodInfo? method;
        ParameterInfo[] pars;
        var type = typeof(RType01A);

        method = type.GetMethod("WithName");
        pars = method!.GetParameters();
        Assert.True(method.IsVirtual);
        Assert.Equal(type, method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);

        method = type.GetMethod("WithAge");
        pars = method!.GetParameters();
        Assert.True(method.IsVirtual);
        Assert.Equal(type, method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Default case inheritance...
    [InheritWiths]
    partial class RType01B : RType01A
    {
        public RType01B(string name) : base(name) { }
        protected RType01B(RType01B source) : base(source) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Type01B()
    {
        MethodInfo? method;
        ParameterInfo[] pars;
        var type = typeof(RType01B);

        method = type.GetMethod("WithName");
        pars = method!.GetParameters();
        Assert.True(method.IsVirtual);
        Assert.Equal(type, method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);

        method = type.GetMethod("WithAge");
        pars = method!.GetParameters();
        Assert.True(method.IsVirtual);
        Assert.Equal(type, method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Default with UseVirtual...
    partial class RType02A
    {
        public RType02A(string name) => Name = name;
        protected RType02A(RType02A source) => Name = source.Name;
        [With(UseVirtual = false)] public string? Name { get; init; }
    }

    //[Enforced]
    [Fact]
    public static void Test_Type02A()
    {
        MethodInfo? method;
        ParameterInfo[] pars;
        var type = typeof(RType02A);

        method = type.GetMethod("WithName");
        pars = method!.GetParameters();
        Assert.False(method.IsVirtual);
        Assert.Equal(type, method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Default UseVirtual inheritance...
    [InheritWiths]
    partial class RType02B : RType02A
    {
        public RType02B(string name) : base(name) { }
        protected RType02B(RType02B source) : base(source) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Type02B()
    {
        MethodInfo? method;
        ParameterInfo[] pars;
        var type = typeof(RType02B);

        method = type.GetMethod("WithName");
        pars = method!.GetParameters();
        Assert.False(method.IsVirtual);
        Assert.Equal(type, method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Inheriting from interface, and return type for inherited...
    partial interface IFace03A { [With] string? Name { get; } }

    [InheritWiths<IFace03A>]
    partial class RType03A : IFace03A
    {
        public RType03A(string name) => Name = name;
        protected RType03A(RType03A source) => Name = source.Name;

        public string? Name { get; init; }
        [With] public int Age = 0;
    }

    //[Enforced]
    [Fact]
    public static void Test_Type03A()
    {
        MethodInfo? method;
        ParameterInfo[] pars;
        var type = typeof(RType03A);

        method = type.GetMethod("WithName");
        pars = method!.GetParameters();
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace03A), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);

        method = type.GetMethod("WithAge");
        pars = method!.GetParameters();
        Assert.True(method.IsVirtual);
        Assert.Equal(type, method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Double inheritance from interface and base class...
    [InheritWiths] partial interface IFace04A : IFace03A { }

    [InheritWiths]
    partial class RType04A : RType03A, IFace04A
    {
        public RType04A(string name) : base(name) { }
        protected RType04A(RType04A source) : base(source) { }
    }

    /* Note: If we specify that 'RType04A' inherits with-members with return type 'IFace03A', what
     * happens is that we have a base method that returns the class 'RType03', with a generated one
     * that returns that 'IFace03A' interface - so a kind-of downcast scenario not supported by the
     * compiler.*/

    //[Enforced]
    [Fact]
    public static void Test_Type04A()
    {
        MethodInfo? method;
        ParameterInfo[] pars;
        var type = typeof(RType04A);

        method = type.GetMethod("WithName");
        pars = method!.GetParameters();
        Assert.True(method.IsVirtual);
        Assert.Equal(type, method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);

        method = type.GetMethod("WithAge");
        pars = method!.GetParameters();
        Assert.True(method.IsVirtual);
        Assert.Equal(type, method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }



    // ----------------------------------------------------

    // Inheriting from interface and abstract...
    partial interface IFace05A { [With] string? Name { get; } }

    [InheritWiths<IFace05A>]
    abstract partial class AType05A : IFace05A { public string? Name { get; init; } }

    [InheritWiths<IFace05A>]
    partial class RType05A : AType05A
    {
        public RType05A(string name) => Name = name;
        protected RType05A(RType05A source) => Name = source.Name;
    }

    //[Enforced]
    [Fact]
    public static void Test_Type05A()
    {
        MethodInfo? method;
        ParameterInfo[] pars;
        var type = typeof(RType05A);

        method = type.GetMethod("WithName");
        pars = method!.GetParameters();
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace05A), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);
    }
}