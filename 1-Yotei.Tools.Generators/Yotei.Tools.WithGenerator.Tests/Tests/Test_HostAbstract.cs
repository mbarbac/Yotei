using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.Tools.WithGenerator.Tests.HostAbstract
{
    // Nested elements...
    //public static partial class TOther
    //{
    //    public abstract partial class AType00
    //    {
    //        public AType00(string name, int age) { Name = name; Age = age; }
    //        protected AType00(AType00 source) { Name = source.Name; Age = source.Age; }
    //        public override string ToString() => $"{Name}, {Age}";
    //        [With] public string Name { get; set; }
    //        [With] public int Age;
    //    }
    //}

    //public static partial class Tests
    //{
    //    //[Enforced]
    //    [Fact]
    //    public static void Test_AType00()
    //    {
    //        var type = typeof(TOther.AType00);

    //        var method = type.GetMethod("WithName");
    //        Assert.NotNull(method);
    //        Assert.True(method.IsAbstract);
    //        Assert.Equal(type, method.ReturnType);
    //        Assert.Single(method.GetParameters());
    //        Assert.Equal(typeof(string), method.GetParameters()[0].ParameterType);

    //        method = type.GetMethod("WithAge");
    //        Assert.NotNull(method);
    //        Assert.True(method.IsAbstract);
    //        Assert.Equal(type, method.ReturnType);
    //        Assert.Single(method.GetParameters());
    //        Assert.Equal(typeof(int), method.GetParameters()[0].ParameterType);
    //    }
    //}

    // ----------------------------------------------------

    // VirtualMethod no effect on abstract...
    //public abstract partial class AType01
    //{
    //    public AType01(string name, int age) { Name = name; Age = age; }
    //    protected AType01(AType01 source) { Name = source.Name; Age = source.Age; }
    //    public override string ToString() => $"{Name}, {Age}";
    //    [With(VirtualMethod = false)] public string Name { get; set; }
    //    [With(VirtualMethod = false)] public int Age;
    //}

    //public static partial class Tests
    //{
    //    //[Enforced]
    //    [Fact]
    //    public static void Test_AType01()
    //    {
    //        var type = typeof(AType01);

    //        var method = type.GetMethod("WithName");
    //        Assert.NotNull(method);
    //        Assert.True(method.IsAbstract);
    //        Assert.Equal(type, method.ReturnType);
    //        Assert.Single(method.GetParameters());
    //        Assert.Equal(typeof(string), method.GetParameters()[0].ParameterType);

    //        method = type.GetMethod("WithAge");
    //        Assert.NotNull(method);
    //        Assert.True(method.IsAbstract);
    //        Assert.Equal(type, method.ReturnType);
    //        Assert.Single(method.GetParameters());
    //        Assert.Equal(typeof(int), method.GetParameters()[0].ParameterType);
    //    }
    //}

    // ----------------------------------------------------

    // Abstract inherits from abstract...
    [InheritWiths]
    public abstract partial class AType02 : AType01
    {
        public AType02(string name, int age) : base(name, age) { }
        protected AType02(AType02 source) : base(source) { }
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_AType02()
        {
            var type = typeof(AType02);

            var method = type.GetMethod("WithName");
            Assert.NotNull(method);
            Assert.True(method.IsAbstract);
            Assert.Equal(type, method.ReturnType);
            Assert.Single(method.GetParameters());
            Assert.Equal(typeof(string), method.GetParameters()[0].ParameterType);

            method = type.GetMethod("WithAge");
            Assert.NotNull(method);
            Assert.True(method.IsAbstract);
            Assert.Equal(type, method.ReturnType);
            Assert.Single(method.GetParameters());
            Assert.Equal(typeof(int), method.GetParameters()[0].ParameterType);
        }
    }

    // ----------------------------------------------------

    // Concrete inherits from abstract...
    //[InheritWiths]
    //public partial class CType02 : AType02
    //{
    //    public CType02(string name, int age) : base(name, age) { }
    //    protected CType02(CType02 source) : base(source) { }
    //}

    //public static partial class Tests
    //{
    //    //[Enforced]
    //    [Fact]
    //    public static void Test_CType02()
    //    {
    //        var type = typeof(CType02);

    //        var method = type.GetMethod("WithName");
    //        Assert.NotNull(method);
    //        Assert.True(method.IsVirtual);
    //        Assert.Equal(type, method.ReturnType);
    //        Assert.Single(method.GetParameters());
    //        Assert.Equal(typeof(string), method.GetParameters()[0].ParameterType);

    //        method = type.GetMethod("WithAge");
    //        Assert.NotNull(method);
    //        Assert.True(method.IsVirtual);
    //        Assert.Equal(type, method.ReturnType);
    //        Assert.Single(method.GetParameters());
    //        Assert.Equal(typeof(int), method.GetParameters()[0].ParameterType);

    //        var source = new CType02("Bond", 50);
    //        var target = source.WithName("James");
    //        Assert.NotSame(source, target);
    //        Assert.Equal("James", target.Name);

    //        target = source.WithAge(100);
    //        Assert.NotSame(source, target);
    //        Assert.Equal(100, target.Age);
    //    }
    //}

    // ----------------------------------------------------

    // New member changing type...
    [InheritWiths]
    public partial class AType03 : AType02
    {
        public AType03(string name, int age) : base(name, age) { }
        protected AType03(AType03 source) : base(source) { }

        [With] public new long Age;
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_AType03()
        {
            var type = typeof(AType03);

            var method = type.GetMethod("WithName");
            Assert.NotNull(method);
            Assert.True(method.IsAbstract);
            Assert.Equal(type, method.ReturnType);
            Assert.Single(method.GetParameters());
            Assert.Equal(typeof(string), method.GetParameters()[0].ParameterType);

            method = type.GetMethod("WithAge");
            Assert.NotNull(method);
            Assert.True(method.IsAbstract);
            Assert.Equal(type, method.ReturnType);
            Assert.Single(method.GetParameters());
            Assert.Equal(typeof(long), method.GetParameters()[0].ParameterType);
        }
    }
}