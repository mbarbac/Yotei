/*using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.Tools.WithGenerator.Tests.HostInterface
{
    // Nested elements...
    public partial interface IOther
    {
        public partial interface IFace00
        {
            [With] string Name { get; }
        }
    }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_IFace00()
        {
            var type = typeof(IOther.IFace00);
            var method = type.GetMethod("WithName");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.Single(method.GetParameters());
            Assert.Equal(typeof(string), method.GetParameters()[0].ParameterType);
        }
    }

    // ----------------------------------------------------

    // VirtualMethod no effect on interfaces...
    public partial interface IFace01 { [With(VirtualMethod = false)] int Age { get; } }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_IFace01()
        {
            var type = typeof(IFace01);
            var method = type.GetMethod("WithAge");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.Single(method.GetParameters());
            Assert.Equal(typeof(int), method.GetParameters()[0].ParameterType);
        }
    }

    // ----------------------------------------------------

    // Duplicated member changing type...
    public partial interface IFace02A { [With] int Age { get; } }
    public partial interface IFace02B : IFace02A { [With] new long Age { get; } }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_IFace02()
        {
            var type = typeof(IFace02A);
            var method = type.GetMethod("WithAge");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.Single(method.GetParameters());
            Assert.Equal(typeof(int), method.GetParameters()[0].ParameterType);

            type = typeof(IFace02B);
            method = type.GetMethod("WithAge");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.Single(method.GetParameters());
            Assert.Equal(typeof(long), method.GetParameters()[0].ParameterType);
        }
    }

    // ----------------------------------------------------

    // Duplicated member same type...
    public partial interface IFace03A { [With] int Age { get; } }
    public partial interface IFace03B : IFace03A { [With] new int Age { get; } }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_IFace03()
        {
            var type = typeof(IFace03A);
            var method = type.GetMethod("WithAge");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.Single(method.GetParameters());
            Assert.Equal(typeof(int), method.GetParameters()[0].ParameterType);

            type = typeof(IFace03B);
            method = type.GetMethod("WithAge");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(type, method.ReturnType);
            Assert.Single(method.GetParameters());
            Assert.Equal(typeof(int), method.GetParameters()[0].ParameterType);
        }
    }

    // ----------------------------------------------------

    // Using return type...
    public partial interface IFace04A { }
    public partial interface IFace04B { [With<IFace04A>] int Age { get; } }

    public static partial class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_IFace04()
        {
            var type = typeof(IFace04B);
            var method = type.GetMethod("WithAge");
            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
            Assert.Equal(typeof(IFace04A), method.ReturnType);
            Assert.Single(method.GetParameters());
            Assert.Equal(typeof(int), method.GetParameters()[0].ParameterType);
        }
    }
}*/