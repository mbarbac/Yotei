#pragma warning disable IDE0065

using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.Tools.CloneGenerator.Tests.Concretes
{
    using IFaces;
    using TTypes;

    // ====================================================
    namespace IFaces
    {
        public partial interface IOther
        {
            // --------------------------------------------
            [Cloneable]
            public partial interface IFoo<T>
            {
                string Name { get; }
            }

            // --------------------------------------------
            [Cloneable(AddICloneable = true)]
            public partial interface IBar<K, T> : IFoo<T>
            {
                string Branch { get; }
            }
        }
    }

    // ====================================================
    namespace TTypes
    {
        public partial class TOther
        {
            // ---------------------------------------------
            [Cloneable]
            public partial class Foo<T> : IOther.IFoo<T>
            {
                public Foo(string name) => Name = name;
                protected Foo(Foo<T> source) : this(source.Name) { }
                public override string ToString() => Name ?? "-";
                public string Name { get; set; }
            }

            // ---------------------------------------------
            [Cloneable(PreventVirtual = true)]
            public partial class Bar<K, T> : Foo<T>, IOther.IBar<K, T>
            {
                public Bar(string name, string branch) : base(name) => Branch = branch;
                protected Bar(Bar<K, T> source) : base(source) => Branch = source.Branch;
                public override string ToString() => $"{base.ToString()}:{Branch ?? "-"}";
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
        public static void Test_Foo_HasCloneMethod()
        {
            var type = typeof(TOther.Foo<int>);
            var method = type.GetMethods().FirstOrDefault(x =>
                x.Name == "Clone" &&
                x.GetParameters().Length == 0);

            Assert.NotNull(method);
            Assert.True(method.IsVirtual);
        }

        //[Enforced]
        [Fact]
        public static void Test_Foo_HasNot_ICloneable()
        {
            var type = typeof(TOther.Foo<int>);
            var iface = typeof(ICloneable);

            try { var map = type.GetInterfaceMap(iface); Assert.Fail(); }
            catch (ArgumentException ex) when (ex.Message == "Interface not found.") { }
        }

        //[Enforced]
        [Fact]
        public static void Test_Foo_Cloning()
        {
            var source = new TOther.Foo<int>("Bond");
            var target = source.Clone();

            Assert.NotSame(target, source);
            Assert.Equal("Bond", target.Name);
        }

        // --------------------------------------------

        //[Enforced]
        [Fact]
        public static void Test_Bar_HasCloneMethod()
        {
            var type = typeof(TOther.Bar<int, int>);
            var method = type.GetMethods().FirstOrDefault(x =>
                x.Name == "Clone" &&
                x.GetParameters().Length == 0);

            Assert.NotNull(method);
            Assert.False(method.IsVirtual); // We've used 'PreventVirtual'
        }

        //[Enforced]
        [Fact]
        public static void Test_Bar_Has_ICloneable_Via_IBar()
        {
            var type = typeof(TOther.Bar<int, int>);
            var iface = typeof(ICloneable);

            type.GetInterfaceMap(iface);
        }

        //[Enforced]
        [Fact]
        public static void Test_Bar_Cloning()
        {
            var source = new TOther.Bar<int, int>("Bond", "MI5");
            var target = source.Clone();

            Assert.NotSame(target, source);
            Assert.Equal("Bond", target.Name);
            Assert.Equal("MI5", target.Branch);
        }
    }
}