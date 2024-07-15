#pragma warning disable IDE0065

namespace Yotei.Tools.CloneGenerator.Tests.AbstractHost
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
            public partial class ConcreteFoo<T> : IOther.IFoo<T>
            {
                public ConcreteFoo(string name) => Name = name;
                protected ConcreteFoo(ConcreteFoo<T> source) : this(source.Name) { }
                public override string ToString() => Name ?? "-";
                public string Name { get; set; }
            }

            // ---------------------------------------------
            [Cloneable]
            public abstract partial class AbstractFoo<T> : IOther.IFoo<T>
            {
                public AbstractFoo(string name) => Name = name;
                protected AbstractFoo(AbstractFoo<T> source) : this(source.Name) { }
                public override string ToString() => Name ?? "-";
                public string Name { get; set; }
            }

            // ---------------------------------------------
            [Cloneable]
            public abstract partial class AbstractBarFromConcrete<K, T> : ConcreteFoo<T>, IOther.IBar<K, T>
            {
                public AbstractBarFromConcrete(string name, string branch) : base(name) => Branch = branch;
                protected AbstractBarFromConcrete(AbstractBarFromConcrete<K, T> source) : base(source) => Branch = source.Branch;
                public override string ToString() => $"{base.ToString()}:{Branch ?? "-"}";
                public string Branch { get; set; }
            }

            // ---------------------------------------------
            [Cloneable]
            public abstract partial class AbstractBarFromAbstract<K, T> : AbstractFoo<T>, IOther.IBar<K, T>
            {
                public AbstractBarFromAbstract(string name, string branch) : base(name) => Branch = branch;
                protected AbstractBarFromAbstract(AbstractBarFromAbstract<K, T> source) : base(source) => Branch = source.Branch;
                public override string ToString() => $"{base.ToString()}:{Branch ?? "-"}";
                public string Branch { get; set; }
            }

            // ---------------------------------------------
            [Cloneable]
            public partial class ConcreteBar<K, T> : AbstractBarFromAbstract<K, T>
            {
                public ConcreteBar(string name, string branch) : base(name, branch) { }
                protected ConcreteBar(ConcreteBar<K, T> source) : base(source) { }
            }
        }
    }

    // ====================================================
    [Enforced]
    public static class Tests
    {
        //[Enforced]
        [Fact]
        public static void Test_ConcreteBar()
        {
            var source = new TOther.ConcreteBar<int, string>("Bond", "MI5");
            var target = source.Clone();
            Assert.NotSame(source, target);
            Assert.Equal("Bond", target.Name);
            Assert.Equal("MI5", target.Branch);
        }

        //[Enforced]
        [Fact]
        public static void Test_AbstractBar()
        {
            TOther.AbstractBarFromAbstract<int, string> source = new TOther.ConcreteBar<int, string>("Bond", "MI5");
            var target = source.Clone();
            Assert.NotSame(source, target);
            Assert.Equal("Bond", target.Name);
            Assert.Equal("MI5", target.Branch);
        }

        //[Enforced]
        [Fact]
        public static void Test_InterfaceBar()
        {
            IOther.IBar<int, string> source = new TOther.ConcreteBar<int, string>("Bond", "MI5");
            var target = source.Clone();
            Assert.NotSame(source, target);
            Assert.Equal("Bond", target.Name);
            Assert.Equal("MI5", target.Branch);
        }
    }
}