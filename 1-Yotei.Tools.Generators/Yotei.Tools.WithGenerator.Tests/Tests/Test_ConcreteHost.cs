#pragma warning disable IDE0065

using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.Tools.WithGenerator.Tests.Concretes
{
    using IFaces;
    using TTypes;

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
    namespace TTypes
    {
        public partial class TOther
        {
            // --------------------------------------------
            [With(InheritMembers = true)]
            public partial class Foo<T> : IFoo<T>
            {
                public Foo(string name) => Name = name;
                protected Foo(Foo<T> source) : this(source.Name) { }
                public override string ToString() => Name ?? "-";
                public string Name { get; set; }
            }

            // --------------------------------------------
            [With(InheritMembers = true)]
            public partial class Bar<K, T> : Foo<T>, IBar<K, T>
            {
                public Bar(string name, string branch) : base(name) => Branch = branch;
                protected Bar(Bar<K, T> source) : this(source.Name, source.Branch) { }
                public override string ToString() => $"{(Name ?? "-")}, {(Branch ?? "-")}";

                [With(PreventVirtual = true)]
                public string Branch { get; set; }
            }
        }
    }

    // ====================================================
    //[Enforced]
    public static class Test
    {
        //[Enforced]
        [Fact]
        public static void Test_Foo_HasMethod()
        {
            var type = typeof(TOther.Foo<int>);
            var method = type.GetMethods().FirstOrDefault(x =>
                x.Name == "WithName" &&
                x.GetParameters().Length == 1 &&
                x.GetParameters()[0].ParameterType == typeof(string));

            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
        }

        //[Enforced]
        [Fact]
        public static void Test_Bar_HasMethods()
        {
            var type = typeof(TOther.Bar<int, int>);
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
            Assert.False(method.IsVirtual); // PreventVirtual requested
        }

        // ------------------------------------------------

        //[Enforced]
        [Fact]
        public static void Test_Foo_Cloning()
        {
            var source = new TOther.Foo<int>("Bond");
            var target = source.WithName("James");
            Assert.NotSame(source, target);
            Assert.Equal("James", target.Name);
        }

        //[Enforced]
        [Fact]
        public static void Test_Bar_Cloning()
        {
            var source = new TOther.Bar<int, int>("Bond", "MI5");
            var target = source.WithBranch("MI6");
            Assert.NotSame(source, target);
            Assert.Equal("Bond", target.Name);
            Assert.Equal("MI6", target.Branch);
        }
    }
}