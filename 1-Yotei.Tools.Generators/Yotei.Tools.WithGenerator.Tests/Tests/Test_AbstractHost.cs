#pragma warning disable IDE0065

using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.Tools.WithGenerator.Tests.Abstracts
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
        [InheritWiths]
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
            [InheritWiths]
            public abstract partial class AbstractFoo<T> : IFoo<T>
            {
                public AbstractFoo(string name) => Name = name;
                protected AbstractFoo(AbstractFoo<T> source) : this(source.Name) { }
                public override string ToString() => Name ?? "-";
                public string Name { get; set; }
            }

            // --------------------------------------------

            [InheritWiths]
            public abstract partial class AbstractFromAbstract<K, T> : AbstractFoo<T>, IBar<K, T>
            {
                public AbstractFromAbstract(string name, string branch) : base(name) => Branch = branch;
                protected AbstractFromAbstract(AbstractFromAbstract<K, T> source) : this(source.Name, source.Branch) { }
                public override string ToString() => $"{(Name ?? "-")}, {(Branch ?? "-")}";
                public string Branch { get; set; }
            }

            // --------------------------------------------
            [InheritWiths]
            public partial class ConcreteFoo<T> : IFoo<T>
            {
                public ConcreteFoo(string name) => Name = name;
                protected ConcreteFoo(ConcreteFoo<T> source) : this(source.Name) { }
                public override string ToString() => Name ?? "-";
                public string Name { get; set; }
            }

            // --------------------------------------------
            [InheritWiths]
            public abstract partial class AbstractFromConcrete<K, T> : ConcreteFoo<T>, IBar<K, T>
            {
                public AbstractFromConcrete(string name, string branch) : base(name) => Branch = branch;
                protected AbstractFromConcrete(AbstractFromConcrete<K, T> source) : this(source.Name, source.Branch) { }
                public override string ToString() => $"{(Name ?? "-")}, {(Branch ?? "-")}";
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
        public static void Test_AbstractFoo_HasMethod()
        {
            var type = typeof(TOther.AbstractFoo<int>);
            var method = type.GetMethods().FirstOrDefault(x =>
                x.Name == "WithName" &&
                x.GetParameters().Length == 1 &&
                x.GetParameters()[0].ParameterType == typeof(string));

            Assert.NotNull(method);
            Assert.True(method.IsAbstract);
        }

        // ------------------------------------------------

        //[Enforced]
        [Fact]
        public static void Test_AbstractFromAbstract_HasMethods()
        {
            var type = typeof(TOther.AbstractFromAbstract<int, int>);
            var method = type.GetMethods().FirstOrDefault(x =>
                x.Name == "WithName" &&
                x.GetParameters().Length == 1 &&
                x.GetParameters()[0].ParameterType == typeof(string));

            Assert.NotNull(method);
            Assert.True(method.IsAbstract);

            method = type.GetMethods().FirstOrDefault(x =>
                x.Name == "WithBranch" &&
                x.GetParameters().Length == 1 &&
                x.GetParameters()[0].ParameterType == typeof(string));

            Assert.NotNull(method);
            Assert.True(method.IsAbstract);
        }

        // ------------------------------------------------

        //[Enforced]
        [Fact]
        public static void Test_ConcreteFoo_HasWithMethod()
        {
            var type = typeof(TOther.ConcreteFoo<int>);
            var method = type.GetMethods().FirstOrDefault(x =>
                x.Name == "WithName" &&
                x.GetParameters().Length == 1 &&
                x.GetParameters()[0].ParameterType == typeof(string));

            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
        }

        // ------------------------------------------------

        //[Enforced]
        [Fact]
        public static void Test_AbstractFromConcrete_HasMethods()
        {
            var type = typeof(TOther.AbstractFromConcrete<int, int>);
            var method = type.GetMethods().FirstOrDefault(x =>
                x.Name == "WithName" &&
                x.GetParameters().Length == 1 &&
                x.GetParameters()[0].ParameterType == typeof(string));

            Assert.NotNull(method);
            Assert.True(method.IsAbstract);

            method = type.GetMethods().FirstOrDefault(x =>
                x.Name == "WithBranch" &&
                x.GetParameters().Length == 1 &&
                x.GetParameters()[0].ParameterType == typeof(string));

            Assert.NotNull(method);
            Assert.True(method.IsAbstract);
        }
    }
}