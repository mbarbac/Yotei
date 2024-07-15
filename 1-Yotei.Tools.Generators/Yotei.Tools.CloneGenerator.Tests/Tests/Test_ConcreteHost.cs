#pragma warning disable IDE0065

namespace Yotei.Tools.CloneGenerator.Tests.ConcreteHost
{
    using IFaces;
    using TTypes;

    // ====================================================
    namespace IFaces
    {
        public partial interface IOther
        {
            // ---------------------------------------------
            [Cloneable]
            public partial interface IFoo<T>
            {
                string Name { get; }
            }

            // ---------------------------------------------
            [Cloneable]
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
            [Cloneable]
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
    [Enforced]
    public static class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_Foo()
        {
            var source = new TOther.Foo<int>("Bond");
            var target = source.Clone();
            Assert.NotSame(source, target);
            Assert.Equal("Bond", target.Name);
        }

        //[Enforced]
        [Fact]
        public static void Test_Bar()
        {
            var source = new TOther.Bar<int, string>("Bond", "MI5");
            var target = source.Clone();
            Assert.NotSame(source, target);
            Assert.Equal("Bond", target.Name);
            Assert.Equal("MI5", target.Branch);
        }
    }
}