#pragma warning disable IDE0065

using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;
/*
namespace Yotei.Tools.WithGenerator.Tests.Interfaces
{
    using IFaces;

    // ====================================================
    namespace IFaces
    {
        // --------------------------------------------
        public partial interface IFoo<T>
        {
            [With] string Name { get; }
        }

        // --------------------------------------------
        [With(InheritMembers = true)]
        public partial interface IBar<K, T> : IFoo<T>
        {
            [With(PreventVirtual = true)] string Branch { get; }
        }
    }

    // ====================================================
    //[Enforced]
    public static class Test
    {
        //[Enforced]
        [Fact]
        public static void Test_IFoo_Methods()
        {
            var type = typeof(IFoo<int>);
            var method = type.GetMethods().FirstOrDefault(x =>
                x.Name == "WithName" &&
                x.GetParameters().Length == 1 &&
                x.GetParameters()[0].ParameterType == typeof(string));

            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
        }

        //[Enforced]
        [Fact]
        public static void Test_IBar_Methods()
        {
            var type = typeof(IBar<int, int>);
            var method = type.GetMethods().FirstOrDefault(x =>
                x.Name == "WithName" &&
                x.GetParameters().Length == 1 &&
                x.GetParameters()[0].ParameterType == typeof(string));

            Assert.NotNull(method);
            Assert.True(method.IsVirtual);

            method = type.GetMethods().FirstOrDefault(x =>
                x.Name == "WithBranch" &&
                x.GetParameters().Length == 1 &&
                x.GetParameters()[0].ParameterType == typeof(string));

            Assert.NotNull(method);
            Assert.True(method.IsVirtual); // PreventVirtual has no effect on interfaces
        }
    }
}*/